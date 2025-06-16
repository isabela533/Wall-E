# 🤖 Wall-E: Intérprete Visual de Pseudocódigo

Este proyecto simula un robot llamado **Wall-E**, capaz de interpretar y ejecutar instrucciones escritas en un pseudolenguaje diseñado para tareas gráficas. El objetivo es mezclar **programación visual**, **lógica estructurada**, y una interfaz amigable desarrollada con **Avalonia UI**.

---

## ✨ Características

- 🧠 Interprete de instrucciones como `Spawn`, `DrawLine`, `Color`, `GoTo`, y más.
- 🖥️ Interfaz gráfica interactiva con canvas donde Wall-E se mueve y pinta.
- 🎨 Soporte para cambio de color, grosor del pincel, y relleno.
- 🧾 Soporte para estructuras de control como ciclos y condicionales.
- 📦 Arquitectura separada por capas: lógica, parser, UI.

---

## 🛠️ Tecnologías utilizadas

- **C#** como lenguaje principal.
- **Avalonia UI** para la interfaz gráfica.
- **.NET** como plataforma base.


---

## 🚀 ¿Cómo funciona?

1. El usuario escribe un programa de instrucciones.
2. El intérprete lo convierte a objetos `IInstruction`.
3. Wall-E interpreta línea por línea y pinta sobre el canvas según los comandos.
