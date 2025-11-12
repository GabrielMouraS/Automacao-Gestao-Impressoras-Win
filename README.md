# 🖨️ Gerenciador de Impressoras — WinForms C#

Aplicativo em C# (Windows Forms) para **listar e remover impressoras em massa** de forma simples e rápida. 

---

## 🎯 Objetivo
Facilitar a **remoção de várias impressoras de uma vez**, sem precisar abrir o Painel de Controle ou executar comandos manualmente.

---
## Imagens do Programa

<img width="482" height="439" alt="image" src="https://github.com/user-attachments/assets/157dc598-828f-404a-a69b-42f052cbadec" />

---

## ⚙️ Funcionalidades
✅ Lista todas as impressoras instaladas no sistema.  
✅ Permite **selecionar múltiplas impressoras** para exclusão.  
✅ Opção de **Selecionar / Desmarcar todas**.  
✅ Usa dois métodos de remoção:
- 🔹 **WMI** (`Win32_Printer.Delete()`), quando disponível.  
- 🔹 **Fallback automático** com `rundll32 printui.dll,PrintUIEntry`, para casos em que o método não é suportado (como OneNote, XPS ou PDF virtual).  
✅ Interface fixa (não redimensionável).  
✅ Sem instalação — basta executar o `.exe`.

---

## 💻 Requisitos
- 🪟 Windows 7, 8, 10 ou 11  
- 🔧 .NET Framework 4.5 ou superior  
- 🧰 Permissão de **Administrador** para remover impressoras  

---

## 🚀 Instalação / Compilação

1. Abra o projeto no **Visual Studio 2015** (ou superior).  
2. Vá em `Project → Properties → Application` e defina o ícone (`.ico`).  
3. Selecione o modo **Release** no Gerenciador de Configuração.  
4. Clique em **Build → Rebuild Solution**.  
5. O executável estará em:
