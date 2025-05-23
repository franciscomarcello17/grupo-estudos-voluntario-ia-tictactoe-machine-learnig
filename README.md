# Jogo da Velh-IA - Aprendizado por Reforço

![Logo](public/assets/images/repository_image.png)  

Um projeto do GEPIA (Grupo de Estudos para Inteligência Artificial da Univille) demonstrando Aprendizado por Reforço em um jogo da velha.

## ✅ Checklist de Funcionalidades

### � Componentes do Jogo
- [X] Tabuleiro 3x3 renderizado corretamente
- [ ] Lógica básica do jogo da velha (turnos, vitória/empate)
- [X] Indicador de turno (jogador vs IA)
- [ ] Animação de transição entre jogadas
- [ ] Modal de fim de jogo (vitória/empate)

### 🤖 Sistema de IA
- [ ] Implementação do algoritmo de aprendizado por reforço
- [X] Tabela no banco para armazenar estados e recompensas
- [ ] Mecanismo de exploração vs explotação
- [X] Persistência do modelo aprendido (localStorage)
- [X] Botão para resetar o conhecimento da IA

### 🌐 Interface
- [X] Página "Jogar" para jogar com a IA
- [X] Página "Visualização de Dados" para visualização de jogadas armazenadas
- [X] Página "Sobre o Projeto" com explicações
- [X] Página "Sobre o GEPIA" com informações do grupo
- [X] Menu de navegação funcional
- [ ] Personalização com fotos e logos do GEPIA
- [ ] Design responsivo para mobile/desktop

### 📊 Visualização de Dados
- [X] Visualização da tabela de jogadas aprendidas
- [X] Filtros gerais

### 🛠️ Infraestrutura
- [X] Controle de versão (GitHub)
- [ ] Hospedagem
- [X] Documentação básica
- [X] Licença aberta (MIT)

## 🛠️ Tecnologias Utilizadas

### Backend
- **ASP.NET Core 9** — Framework robusto para desenvolvimento do backend.
- **Entity Framework Core 9** — ORM para acesso e manipulação do banco de dados.
- **SQL Server** — Banco de dados relacional para armazenamento dos modelos de IA e dados do sistema.
- **Swagger** — Ferramenta para documentação e testes das APIs REST.

### Frontend
- **AngularJS** — Framework MVC para construção da interface web dinâmica.
- **JavaScript** — Implementação da lógica do jogo e da inteligência artificial.
- **HTML5 & CSS3** — Estrutura e estilo da interface do usuário.
- **Chart.js** — Biblioteca para visualização gráfica dos dados.

### Controle de Versão
- **Git / GitHub** — Gestão do código-fonte e versionamento colaborativo.

## 🤝 Como Contribuir

1. Faça um fork do projeto
2. Crie sua branch (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

## Como Rodar a Aplicação Localmente

### Rodar o Front-end (AngularJS)

1. Certifique-se de ter o [Node.js](https://nodejs.org/) instalado.
2. Instale o `live-server` globalmente para servir os arquivos estáticos:

   ```bash
   npm install -g live-server
    ```

3. Navegue até a pasta do front-end (exemplo: `public`):

   ```bash
   cd public
   ```

4. Execute o `live-server`, garantindo que a SPA funcione corretamente usando o arquivo de entrada `index.html`:

   ```bash
   live-server --entry-file=index.html
   ```

5. O front-end estará disponível normalmente em `http://127.0.0.1:8080` (ou outra porta que o live-server escolher).

### Rodar o Back-end (API)

1. Abra o terminal na pasta do projeto do back-end.

2. Certifique-se de ter o [.NET 9 SDK](https://dotnet.microsoft.com/download) instalado.

3. Restaure as dependências e rode o projeto:

   ```bash
   dotnet restore
   dotnet run
   ```

4. A API estará disponível no endereço configurado (exemplo: `https://localhost:5001`).

## 📧 Contato

Grupo de Estudos para Inteligência Artificial - Univille  
Email: gepia.univille01gmail.com
GitHub: [github.com/gepia-univille](https://github.com/GepiaUniville)

## 📄 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

---

Desenvolvido com ❤️ pelo GEPIA - 2025
