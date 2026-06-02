<div align="center">
<img width="35%" alt="b751f368-748b-418f-afea-e9f7cd70ab95" src="https://github.com/user-attachments/assets/b04d7d6c-3908-4382-a2e2-f7d415dbe93a" />
  
  # Bonobo Git Server: Bernese Edition 🚀

  
  **El servidor Git privado para Windows e IIS, completamente modernizado y blindado.**
  
  [![Release](https://img.shields.io/github/v/release/knario133/BonovoClone1?color=blue&label=Versi%C3%B3n)](https://github.com/knario133/BonovoClone1/releases/latest)
  [![Platform](https://img.shields.io/badge/Plataforma-Windows%20%7C%20IIS-lightgrey)](#)
  [![Framework](https://img.shields.io/badge/.NET_Framework-4.7.2-512BD4)](#)
  [![License](https://img.shields.io/github/license/knario133/BonovoClone1)](#)
</div>

---

## 📌 ¿Qué es la Bernese Edition?

Esta versión es un fork profundamente refactorizado del clásico [Bonobo Git Server](https://bonobogitserver.com/). La **Bernese Edition** nació con un objetivo claro: erradicar años de deuda técnica, modernizar la arquitectura del motor de base de datos y presentar una interfaz de usuario fluida y a prueba de estrés para equipos de desarrollo modernos.

Si buscas hospedar tus repositorios Git en tus propios servidores Windows con IIS, de forma segura, rápida y con una interfaz de primer nivel, estás en el lugar correcto.

---

## ✨ Características Principales

### ⚙️ Motor de Alto Rendimiento
* **Base de Datos Nativa:** Migración completa a `e_sqlite3` (C++). Adiós a los bloqueos de base de datos (*"Database is locked"*) y a los problemas de arquitectura 32/64 bits.
* **Carga Diferida (Lazy Loading):** Los *diffs* de los commits pesados se cargan asíncronamente (AJAX) bajo demanda, reduciendo el consumo de RAM del servidor en un 90%.
* **Dependencias Modernas:** Ecosistema actualizado a **.NET Framework 4.7.2** con resoluciones estrictas de `assemblyBinding` para un arranque estable.

### 🎨 Interfaz y Experiencia (UI/UX)
* **Tablas Dinámicas (DataTables):** Búsqueda en tiempo real, ordenamiento y paginación en repositorios, archivos, usuarios y commits.
* **Layout Maximizado "Glass/Dark":** Eliminación de espacios muertos, con un menú lateral discreto para aprovechar al máximo los monitores modernos.
* **DOM Controlado:** Barras de desplazamiento encapsuladas en los *diffs* para evitar perder la posición al leer commits kilométricos.
* **Loader Inteligente:** Protege la sesión durante operaciones pesadas sin bloquear la vista en peticiones silenciosas.

### 🔍 Visor de Código Avanzado
* **Sintaxis Nivel IDE:** Integración de `highlight.js` con soporte para C#, JS, TS, XML, JSON y más.
* **Modo Oscuro Integrado:** Para cuidar la vista durante las revisiones de código (*Code Reviews*).
* **Previsualización de Documentos:** Lee PDFs, DOCX y XLSX directamente desde el navegador sin necesidad de descargarlos.

---

## 📸 Galería de la Interfaz

<div align="center">
  <img width="49%" alt="Navegador de Repositorios" src="https://github.com/user-attachments/assets/d34ee94a-86c0-4cf9-aff8-476c33de3c2c" />
  <img width="49%" alt="Modo Oscuro Visor de Código" src="https://github.com/user-attachments/assets/44d16673-1502-4377-b6e4-bd214e3bfe4c" />
</div>
<div align="center">
  <img width="49%" alt="Historial de Commits DataTables" src="https://github.com/user-attachments/assets/0b12615d-9446-4b0d-815e-a680dcdb49c8" />
  <img width="49%" alt="Detalles de Diff Carga Asíncrona" src="https://github.com/user-attachments/assets/7d1b5df0-65fd-4734-8953-b8834a6f721c" />
</div>

---

## 🚀 Guía Rápida de Instalación

1. **Descarga el Release:** Ve a la sección de [Releases](https://github.com/knario133/BonovoClone1/releases) y baja el último `.zip`.
2. **Prepara IIS:** Descomprime los archivos en tu ruta web (ej. `C:\inetpub\wwwroot\BonoboGitServer`).
3. **Application Pool:** Crea un Pool en IIS con `.NET v4.0` en modo `Integrado`.
4. **Permisos (🚨 Crítico):** Otorga permisos de **Modificar** y **Escritura** al usuario `IIS_IUSRS` (o `IUSR`) sobre la carpeta `App_Data`.
5. **Ingresa:** Abre tu navegador. Usuario y contraseña por defecto: `admin` / `admin`. (¡Cámbiala inmediatamente!).

*Para instrucciones detalladas paso a paso, revisa las notas de la versión en la sección de Releases.*

---

## 🛠️ Entorno de Desarrollo (Stack)

* **Backend:** ASP.NET MVC, C#, .NET Framework 4.7.2.
* **Base de datos:** SQLite (Motor nativo `e_sqlite3`).
* **Frontend:** HTML5, CSS3, jQuery, DataTables, Highlight.js.
* **Servidor Destino:** Windows Server / IIS.

---

## 🤝 Contribución y Comunidad

La **Bernese Edition** es una versión *Community Beta* enfocada en estabilidad y rendimiento. Las pruebas de estrés, monitoreo de memoria y reportes de errores (Issues) son bienvenidos. 

Si deseas contribuir, por favor abre un Issue para discutir los cambios propuestos antes de enviar un Pull Request.

---
*Este proyecto está basado en el código original de [Bonobo Git Server](https://github.com/jakubgarfield/Bonobo-Git-Server).*
