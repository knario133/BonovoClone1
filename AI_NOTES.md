# 📖 MANUAL ESTRATÉGICO Y HOJA DE RUTA UI/UX (Bonobo Bank Edition)

## 1. Contexto del Entorno y Restricciones Arquitectónicas
### 1.1. Entorno Corporativo Aislado (Red Bancaria VPN)
#### 1.1.1. Prohibición Absoluta de CDNs
- Ningún archivo CSS, JS, fuente tipográfica o imagen puede ser invocado desde dominios externos (ej. cdnjs, jsdelivr, google fonts). Todo el tráfico externo está bloqueado por el firewall del banco.
#### 1.1.2. Gestión de Activos Estáticos (Local)
- TODAS las librerías (Bootstrap 5, FontAwesome 6, DataTables, Scripts de Fondos) deben ser descargadas y referenciadas localmente.
- Rutas obligatorias: CSS en `~/Content/librerias/...` y JavaScript en `~/Scripts/librerias/...`.
- Si necesitas una librería, tu tarea es crear el archivo físico en el proyecto con el código correspondiente e inyectarlo en el `<head>` o antes de cerrar el `<body>`.

## 2. Reglas Estrictas del Motor de Vistas (ASP.NET MVC Razor)
### 2.1. Gestión de Bloques de Código y Sintaxis
#### 2.1.1. Obligatoriedad del prefijo `@`
- Todo bloque condicional o iterativo en C# DEBE iniciar con `@`. 
- Escribir `if (condición)` sin `@` imprimirá el texto literalmente en el DOM. Forma correcta: `@if (condición)`.
#### 2.1.2. Transiciones de C# a HTML (Cambio de Contexto)
- Si dentro de un bloque `@if { ... }` se requiere renderizar HTML, envuelve el contenido en etiquetas estándar (`<div>`, `<li>`, `<a>`). 
- Razor cambiará a HTML automáticamente. **ESTÁ ESTRICTAMENTE PROHIBIDO** usar la etiqueta `<text>` si ya estás usando elementos HTML válidos.
- Las llaves `{` y `}` de control de flujo en C# jamás deben renderizarse en el navegador. Si ocurre, la estructura está rota.

### 2.2. Preservación del Layout Maestro (`_Layout.cshtml`)
#### 2.2.1. Estructura Dual Obligatoria (Sidebar + Main)
- El Layout debe mantener imperativamente dos grandes bloques bajo un contenedor padre Flexbox (`d-flex`):
  1. `<nav class="sidebar">`: Panel lateral izquierdo, ancho fijo, siempre visible. Contiene el menú y el perfil del usuario.
  2. `<main class="flex-grow-1">`: Panel derecho fluido que contiene el `@RenderBody()`.

## 3. Guía de Estilo Visual (Glassmorphism Dark Mode)
### 3.1. Esquema de Colores y Fondos
#### 3.1.1. Fondo Base (Fallback)
- Un gradiente lineal suave de azul profundo a púrpura muy oscuro.
#### 3.1.2. Fondo Dinámico (Simulado/Local)
- Dado que la API externa de Bing puede estar bloqueada, debes prever una capa oscura (overlay) tipo `background-color: rgba(15, 23, 42, 0.85);` que cubra todo el fondo para garantizar el contraste de lectura.
### 3.2. Paneles, Tarjetas y Sidebar
#### 3.2.1. Efecto Cristal (Backdrop-filter)
- Todo elemento de interfaz elevado (Sidebar, Cards, Modales) debe tener: `background: rgba(0, 0, 0, 0.4);`, `backdrop-filter: blur(12px);`, un borde sutil `border: 1px solid rgba(255, 255, 255, 0.1);` y `box-shadow` profundo.

## 4. Hoja de Ruta Iterativa (Flujo de Trabajo del Agente)
### 4.1. FASE 1: Estabilización del `_Layout.cshtml` (ESTADO ACTUAL: BLOQUEADO/EN CURSO)
#### 4.1.1. Reconstrucción del Menú Lateral
- Debes restaurar el Sidebar que desapareció en iteraciones previas.
- Debe incluir todos los enlaces originales (`@Url.Action`) respetando los condicionales `@if` de autenticación.
#### 4.1.2. Inserción de Librerías Locales
- Asegurar que Bootstrap y FontAwesome se carguen desde rutas relativas locales (`~/Content/...`).
- **META DE FASE:** Renderizar el Sidebar y el RenderBody sin errores de Razor (sin llaves en pantalla). **NO AVANZAR A LA FASE 2 HASTA LOGRARLO.**

### 4.2. FASE 2: Pantalla de Autenticación (`LogOn.cshtml`)
#### 4.2.1. Refactorización del Formulario
- Reemplazar las clases antiguas por inputs modernos de Bootstrap (`form-control`, `form-floating`).
#### 4.2.2. Aplicación de Glassmorphism
- El formulario de login debe estar centrado vertical y horizontalmente dentro de una tarjeta translúcida de cristal.

### 4.3. FASE 3: Dashboard Principal (`Repository/Index.cshtml`)
#### 4.3.1. Integración de DataTables (Local)
- Convertir la tabla HTML clásica en un DataTable responsivo, oscuro y con estilos de Bootstrap 5.
#### 4.3.2. Interactividad Visual
- Añadir efectos *hover* en las filas de la tabla y botones de acción brillantes.
#### 4.3.3. Transparencia Obligatoria en Tablas (Overrides CSS)
- Las librerías como DataTables inyectan fondos sólidos por defecto. Tienes la obligación de inyectar un bloque `<style>` en la vista que fuerce `background-color: transparent !important;` en los elementos `table.dataTable`, `tr`, `th` y `td`.
- Las filas deben tener un efecto hover: `table.dataTable tbody tr:hover { background-color: rgba(255, 255, 255, 0.05) !important; backdrop-filter: blur(5px); }`.
- Los botones de paginación (`.page-link`) y los inputs del buscador deben tener fondo translúcido oscuro (`rgba(0,0,0,0.4)`) y bordes semitransparentes, nunca blanco sólido.
