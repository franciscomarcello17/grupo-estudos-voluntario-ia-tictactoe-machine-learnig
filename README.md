# Jogo da Velha com IA - Aprendizado por Reforço

Um projeto do GEPIA (Grupo de Estudos para Inteligência Artificial da Univille) demonstrando Aprendizado por Reforço em um jogo da velha.

## ✅ Checklist de Funcionalidades

### � Componentes do Jogo
- [ ] Tabuleiro 3x3 renderizado corretamente
- [ ] Lógica básica do jogo da velha (turnos, vitória/empate)
- [ ] Indicador de turno (jogador vs IA)
- [ ] Feedback visual para jogadas válidas/inválidas
- [ ] Animação de transição entre jogadas
- [ ] Modal de fim de jogo (vitória/empate)

### 🤖 Sistema de IA
- [ ] Implementação do algoritmo Q-Learning
- [ ] Tabela Q para armazenar estados e recompensas
- [ ] Mecanismo de exploração vs explotação (ε-greedy)
- [ ] Função de recompensa adequada
- [ ] Persistência do modelo aprendido (localStorage)
- [ ] Botão para resetar o conhecimento da IA

### 🌐 Interface
- [ ] Página "Sobre o Projeto" com explicações
- [ ] Página "Sobre o GEPIA" com informações do grupo
- [ ] Menu de navegação funcional
- [ ] Design responsivo para mobile/desktop
- [ ] Temas claro/escuro (opcional)

### 📊 Visualização de Dados
- [ ] Gráfico de aprendizado ao longo do tempo
- [ ] Estatísticas de vitórias/derrotas/empates
- [ ] Visualização da tabela Q (simplificada)

### 🛠️ Infraestrutura
- [ ] Controle de versão (Git)
- [ ] Hospedagem em GitHub Pages
- [ ] Documentação básica
- [ ] Licença aberta

## 🚀 Como Executar

1. Clone o repositório:
```bash
git clone https://github.com/gepia-univille/jogo-da-velha-ia.git
```

2. Abra o arquivo `index.html` no seu navegador.

## 🧠 Sobre a IA

A IA utiliza **Aprendizado por Reforço** com os seguintes parâmetros:
- Taxa de aprendizado (α): 0.3
- Fator de desconto (γ): 0.9
- Probabilidade de exploração (ε): 0.2 (decai com o tempo)

```javascript
// Exemplo da estrutura da tabela Q
{
  "state": "X--------",
  "actions": {
    "0": 0.5, // Posição 0 com valor Q atual
    "1": 0.3,
    // ...
  }
}
```

## 🛠️ Tecnologias Utilizadas

- AngularJS (para estrutura MVC)
- JavaScript (lógica do jogo e IA)
- HTML5/CSS3 (interface)
- Chart.js (visualização de dados)
- Git/GitHub (controle de versão)

## 🤝 Como Contribuir

1. Faça um fork do projeto
2. Crie sua branch (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

## 📧 Contato

Grupo de Estudos para Inteligência Artificial - Univille  
Email: gepia@univille.edu.br  
GitHub: [github.com/gepia-univille](https://github.com/gepia-univille)

## 📄 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

---

Desenvolvido com ❤️ pelo GEPIA - 2025
```