@echo off
chcp 65001 > nul
echo ========================================================
echo        INICIANDO SUBIDA DE LA BERNESE EDITION
echo ========================================================
echo.

:: 1. Crear el archivo temporal con el Super Commit
echo Generando bitacora de cambios...
echo Refactor: Modernización integral de infraestructura, UI/UX y rendimiento (Bernese Edition) > commit_msg.txt
echo. >> commit_msg.txt
echo Esta actualización representa una refactorización masiva del sistema, eliminando deuda técnica crítica en el motor de base de datos, optimizando el consumo de memoria del servidor mediante carga bajo demanda, y modernizando por completo la interfaz de usuario con un visor de código avanzado y tablas dinámicas. >> commit_msg.txt
echo. >> commit_msg.txt
echo [INFRAESTRUCTURA Y BACKEND] >> commit_msg.txt
echo - Actualización total de dependencias NuGet alineadas a .NET Framework 4.7.2. >> commit_msg.txt
echo - Resolución de conflictos de 'assemblyBinding' y purga de dependencias circulares en Web.config. >> commit_msg.txt
echo - Integración exitosa del motor SQLite nativo, eliminando bloqueos de arquitectura. >> commit_msg.txt
echo - Compilación validada con MSBuild: 0 advertencias, 0 errores. >> commit_msg.txt
echo. >> commit_msg.txt
echo [RENDIMIENTO Y ESTABILIDAD] >> commit_msg.txt
echo - Implementación de "Lazy Loading" (AJAX) para la carga de Diffs en la vista de Commits, evitando colapso de RAM. >> commit_msg.txt
echo - Corrección de bugs críticos de navegación: acceso a ramas y visualización de binarios en vista Raw. >> commit_msg.txt
echo - Solución al error 404 en la renderización de archivos PDF. >> commit_msg.txt
echo - Corrección de errores de ejecución Razor. >> commit_msg.txt
echo. >> commit_msg.txt
echo [INTERFAZ Y NAVEGACION (FRONTEND)] >> commit_msg.txt
echo - Integración global de DataTables para habilitar búsqueda, ordenamiento y paginación en tiempo real. >> commit_msg.txt
echo - Rediseño del Layout (Glass/Dark): Menú lateral discreto y maximización del área de trabajo. >> commit_msg.txt
echo - Implementación de Loader Global con overlay inteligente. >> commit_msg.txt
echo - Contención del DOM en vistas de diffs mediante barras de desplazamiento encapsuladas. >> commit_msg.txt
echo. >> commit_msg.txt
echo [VISOR DE CODIGO AVANZADO] >> commit_msg.txt
echo - Integración local de highlight.js con soporte nativo para C#, JavaScript, TypeScript, XML y JSON. >> commit_msg.txt
echo - Incorporación de Modo Oscuro, búsqueda interna de texto y formateo/reindentación automática. >> commit_msg.txt
echo - Habilitación de previsualización local para documentos de Office y PDFs. >> commit_msg.txt
echo. >> commit_msg.txt
echo [QA y PRUEBAS] >> commit_msg.txt
echo - Validado: Flujo de autenticación, navegación en árbol de archivos, carga asíncrona de parches y estabilidad de IIS. >> commit_msg.txt

:: 2. Ejecutar comandos de Git
echo.
echo [1/3] Agregando archivos al Stage (git add .)...
git add .

echo [2/3] Empaquetando el código (git commit)...
git commit -F commit_msg.txt

echo [3/3] Enviando a la nube (git push origin master)...
git push origin master

:: 3. Limpieza
echo.
echo Limpiando archivos temporales...
del commit_msg.txt

echo.
echo ========================================================
echo        ¡MISION CUMPLIDA! EL CODIGO ESTA A SALVO.
echo ========================================================
pause