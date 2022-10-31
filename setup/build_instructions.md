# Instruções para compilação do programa de instalação

0. Faça o download do [Visual Studio 2022](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&channel=Release&version=VS2022&source=VSLandingPage&cid=2030&passive=false) (com as cargas de trabalho "Desenvolvimento .NET" e "Desenvolvimento Python") e do [Inno Setup Compiler](https://jrsoftware.org/download.php/is.exe) (Disponíveis nos links)
1. Baixe ou clone o repositório em uma pasta `ducreate\`
2. Abra o arquivo `ducreate\Expovgen\Expovgen.sln` no Visual Studio e clique em **Compilação > Compilar Solução**
***NOTA**: Após a compilação, você já pode executar o programa, clicando em **Depurar > Iniciar Depuração** ou **Depurar > Iniciar Sem Depurar***
3. Abra o arquivo `ducreate\setup\build_installer.iss` no Inno Setup e clique em **Build > Compile** ou aperte **Ctrl+F9**
4. **Seu instalador estará na pasta `ducreate\setup\bin`!**