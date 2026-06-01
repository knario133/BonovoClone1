(function () {
    function ready(fn) {
        if (document.readyState !== "loading") {
            fn();
            return;
        }
        document.addEventListener("DOMContentLoaded", fn);
    }

    function escapeRegExp(value) {
        return value.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
    }

    function normalizeExtension(value) {
        return (value || "").toLowerCase().replace(/^\./, "");
    }

    function getPrettierParser(extension) {
        switch (normalizeExtension(extension)) {
            case "js":
            case "jsx":
                return "babel";
            case "json":
                return "json";
            case "ts":
            case "tsx":
                return "typescript";
            case "html":
            case "htm":
            case "xhtml":
            case "xml":
            case "cshtml":
            case "aspx":
            case "config":
            case "csproj":
                return "html";
            case "css":
                return "css";
            case "scss":
                return "scss";
            case "less":
                return "less";
            default:
                return null;
        }
    }

    function formatStructuredText(source, extension) {
        var normalized = normalizeExtension(extension);
        if (!source) {
            return source;
        }

        try {
            if (normalized === "json") {
                if (window.vkbeautify && window.vkbeautify.json) {
                    return window.vkbeautify.json(source, 4);
                }

                return JSON.stringify(JSON.parse(source), null, 4);
            }

            if (normalized === "xml" || normalized === "config" || normalized === "csproj" || normalized === "xaml" || normalized === "xslt") {
                if (window.vkbeautify && window.vkbeautify.xml) {
                    return window.vkbeautify.xml(source, 4);
                }
            }
        } catch (error) {
            return source;
        }

        return source;
    }

    function getPrettierPlugins() {
        if (!window.prettierPlugins) {
            return [];
        }

        return [
            window.prettierPlugins.babel,
            window.prettierPlugins.estree,
            window.prettierPlugins.html,
            window.prettierPlugins.postcss,
            window.prettierPlugins.typescript
        ].filter(Boolean);
    }

    function reindentWithBraces(source) {
        var indent = 0;
        return source.replace(/\t/g, "    ").split(/\r?\n/).map(function (line) {
            var trimmed = line.trim();
            if (!trimmed) {
                return "";
            }

            if (/^[}\])]/.test(trimmed)) {
                indent = Math.max(indent - 1, 0);
            }

            var output = new Array(indent + 1).join("    ") + trimmed;
            var opens = (trimmed.match(/[{\[(]/g) || []).length;
            var closes = (trimmed.match(/[}\])]/g) || []).length;
            if (!/[;,:]$/.test(trimmed) && /^(if|for|foreach|while|switch|using|else|try|catch|finally)\b/.test(trimmed)) {
                opens += 1;
            }
            indent = Math.max(indent + opens - closes, 0);
            return output;
        }).join("\n");
    }

    function updateCode(code, text) {
        code.textContent = text;
        code.removeAttribute("data-highlighted");
        if (window.hljs) {
            window.hljs.highlightElement(code);
        }
        code.dataset.highlightedHtml = code.innerHTML;
    }

    function highlightSearch(code, query) {
        code.innerHTML = code.dataset.highlightedHtml || code.innerHTML;
        if (!query) {
            return [];
        }

        var marks = [];
        var matcher = new RegExp(escapeRegExp(query), "gi");
        var walker = document.createTreeWalker(code, NodeFilter.SHOW_TEXT, null, false);
        var nodes = [];
        var node;
        while ((node = walker.nextNode())) {
            nodes.push(node);
        }

        nodes.forEach(function (textNode) {
            var text = textNode.nodeValue;
            if (!matcher.test(text)) {
                matcher.lastIndex = 0;
                return;
            }
            matcher.lastIndex = 0;
            var fragment = document.createDocumentFragment();
            var cursor = 0;
            text.replace(matcher, function (match, offset) {
                if (offset > cursor) {
                    fragment.appendChild(document.createTextNode(text.slice(cursor, offset)));
                }
                var mark = document.createElement("mark");
                mark.className = "blob-search-hit";
                mark.textContent = match;
                marks.push(mark);
                fragment.appendChild(mark);
                cursor = offset + match.length;
                return match;
            });
            if (cursor < text.length) {
                fragment.appendChild(document.createTextNode(text.slice(cursor)));
            }
            textNode.parentNode.replaceChild(fragment, textNode);
        });

        return marks;
    }

    function setupSearch(viewer) {
        var code = viewer.querySelector("[data-blob-code]");
        var input = viewer.querySelector("[data-blob-search]");
        var next = viewer.querySelector("[data-blob-next]");
        var previous = viewer.querySelector("[data-blob-previous]");
        var count = viewer.querySelector("[data-blob-search-count]");
        if (!code || !input) {
            return;
        }

        var marks = [];
        var active = -1;

        function select(index) {
            marks.forEach(function (mark) { mark.classList.remove("active"); });
            if (!marks.length) {
                active = -1;
                count.textContent = "0/0";
                return;
            }
            active = (index + marks.length) % marks.length;
            marks[active].classList.add("active");
            marks[active].scrollIntoView({ block: "center", inline: "center" });
            count.textContent = (active + 1) + "/" + marks.length;
        }

        function refresh() {
            marks = highlightSearch(code, input.value.trim());
            select(marks.length ? 0 : -1);
        }

        input.addEventListener("input", refresh);
        next && next.addEventListener("click", function () { select(active + 1); });
        previous && previous.addEventListener("click", function () { select(active - 1); });
    }

    function setupReindent(viewer) {
        var code = viewer.querySelector("[data-blob-code]");
        var button = viewer.querySelector("[data-blob-reindent]");
        var status = viewer.querySelector("[data-blob-status]");
        if (!code || !button) {
            return;
        }

        button.addEventListener("click", async function () {
            var original = code.textContent || "";
            var extension = viewer.getAttribute("data-extension");
            var parser = getPrettierParser(extension);
            var formatted = null;

            button.disabled = true;
            if (status) {
                status.textContent = "Formatting...";
            }

            try {
                if (parser && window.prettier) {
                    formatted = await window.prettier.format(original, {
                        parser: parser,
                        plugins: getPrettierPlugins(),
                        tabWidth: 4,
                        useTabs: false
                    });
                }
            } catch (error) {
                formatted = null;
            }

            if (!formatted) {
                formatted = formatStructuredText(original, extension);
            }

            if ((!formatted || formatted === original) && !/^(json|xml|config|csproj|xaml|xslt)$/i.test(normalizeExtension(extension))) {
                formatted = reindentWithBraces(original);
            }

            updateCode(code, formatted);
            if (status) {
                status.textContent = parser ? "Formatted locally" : "Reindented locally";
                window.setTimeout(function () {
                    if (status) {
                        status.textContent = "";
                    }
                }, 2200);
            }
            button.disabled = false;
        });
    }

    function setupDocumentPreview(viewer) {
        var preview = viewer.querySelector("[data-document-preview]");
        if (!preview) {
            return;
        }

        var extension = normalizeExtension(viewer.getAttribute("data-extension"));
        var rawUrl = viewer.getAttribute("data-raw-url");
        if (!rawUrl) {
            return;
        }

        if (extension === "pdf") {
            preview.innerHTML = '<iframe class="document-preview-frame" src="' + rawUrl + '"></iframe>';
            return;
        }

        loadArrayBuffer(rawUrl)
            .then(function (buffer) {
                if (extension === "docx" && window.mammoth) {
                    return window.mammoth.convertToHtml({ arrayBuffer: buffer }).then(function (result) {
                        preview.innerHTML = '<div class="document-preview-html">' + result.value + '</div>';
                    });
                }

                if ((extension === "xlsx" || extension === "xls") && window.XLSX) {
                    var workbook = window.XLSX.read(buffer, { type: "array" });
                    var firstSheet = workbook.Sheets[workbook.SheetNames[0]];
                    preview.innerHTML = '<div class="document-preview-table">' + window.XLSX.utils.sheet_to_html(firstSheet) + '</div>';
                    return;
                }

                preview.innerHTML = '<div class="document-preview-empty">Preview local no disponible para este tipo de documento.</div>';
            })
            .catch(function () {
                preview.innerHTML = '<div class="document-preview-empty">No se pudo preparar la vista previa local.</div>';
            });
    }

    function loadArrayBuffer(url) {
        return new Promise(function (resolve, reject) {
            var request = new XMLHttpRequest();
            request.open("GET", url, true);
            request.responseType = "arraybuffer";
            request.withCredentials = true;
            request.onload = function () {
                if (request.status >= 200 && request.status < 300) {
                    resolve(request.response);
                    return;
                }

                reject(new Error("HTTP " + request.status));
            };
            request.onerror = reject;
            request.send();
        });
    }

    ready(function () {
        var viewer = document.querySelector("[data-blob-viewer]");
        if (!viewer) {
            return;
        }

        var code = viewer.querySelector("[data-blob-code]");
        if (code) {
            updateCode(code, formatStructuredText(code.textContent || "", viewer.getAttribute("data-extension")));
            setupSearch(viewer);
            setupReindent(viewer);
        }

        setupDocumentPreview(viewer);
    });
})();
