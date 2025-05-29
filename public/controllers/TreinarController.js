angular.module('jogoDaVelhaApp').controller('TreinarController', 
    ['$scope', '$timeout', 'ApiService', function($scope, $timeout, ApiService) {
        var vm = this;
        
        // Estado inicial do jogo
        vm.tabuleiro = ["", "", "", "", "", "", "", "", ""];
        vm.jogadorAtual = "X";
        vm.jogadasAprendidas = [];
        vm.carregando = false;
        vm.carregandoJogadas = false;
        vm.mensagem = "";
        vm.mostrarDicas = false;
        vm.filtroTabuleiro = '';
        vm.filtroResultado = '';
        
        // Inicialização
        vm.init = function() {
            carregarJogadas();
        };
        
        // Carrega as jogadas aprendidas da IA
        function carregarJogadas() {
            vm.carregandoJogadas = true;
            ApiService.getAprendizado().then(function(response) {
                vm.jogadasAprendidas = response.data || [];
            }).catch(function(error) {
                console.error("Erro ao carregar jogadas:", error);
                vm.mensagem = "Erro ao carregar histórico de jogadas";
                vm.jogadasAprendidas = [];
            }).finally(function() {
                vm.carregandoJogadas = false;
            });
        }
        
        // Função principal quando o jogador faz uma jogada
        vm.jogar = function(posicao) {
            // Validações básicas
            if (vm.carregando || vm.tabuleiro[posicao] !== "") return;
            
            // Faz a jogada do humano
            vm.tabuleiro[posicao] = 'X';
            // Verifica se o humano venceu
            if (verificarVitoria('X')) {
                finalizarJogo("Vitoria", posicao);
                return;
            }
            
            // Verifica empate
            if (verificarEmpate()) {
                finalizarJogo("Empate", posicao);
                return;
            }
            
            // Passa a vez para a IA
            vm.carregando = true;
            vm.jogadorAtual = 'O';
            realizarJogadaIA();
        };
        
        // Função que processa a jogada da IA
        function realizarJogadaIA() {
            $timeout(function() {
                ApiService.postJogarComBase(vm.tabuleiro.join(","))
                    .then(function(response) {
                        var posicaoIA = response.data.posicaoEscolhida;
                        console.log("Jogada da IA:", posicaoIA);
                        // Fallback para jogada aleatória se necessário
                        if (posicaoIA === -1 || vm.tabuleiro[posicaoIA] !== "") {
                            posicaoIA = escolherJogadaAleatoria();
                        }
                        
                        // Aplica a jogada da IA
                        if (posicaoIA !== -1) {
                            vm.tabuleiro[posicaoIA] = 'O';
                            
                            // Verifica o resultado após jogada da IA
                            if (verificarVitoria('O')) {
                                finalizarJogo("Derrota", posicaoIA);
                            } else if (verificarEmpate()) {
                                finalizarJogo("Empate", posicaoIA);
                            } else {
                                vm.jogadorAtual = 'X';
                            }
                        }
                    })
                    .catch(function(error) {
                        console.error("Erro na IA:", error);
                        // Jogada aleatória em caso de erro (outro fallback)
                        var posicaoIA = escolherJogadaAleatoria();
                        if (posicaoIA !== -1) {
                            vm.tabuleiro[posicaoIA] = 'O';

                            // Verifica o resultado após jogada da IA
                            if (verificarVitoria('O')) {
                                finalizarJogo("Derrota", posicaoIA);
                            } else if (verificarEmpate()) {
                                finalizarJogo("Empate", posicaoIA);
                            } else {
                                vm.jogadorAtual = 'X';
                            }

                        }
                    })
                    .finally(function() {
                        vm.carregando = false;
                    });
            }, 500); // Delay para melhor experiência
        }
        
        // Escolhe uma jogada aleatória para a IA
        function escolherJogadaAleatoria() {
            var posicoesVazias = [];
            for (var i = 0; i < 9; i++) {
                if (vm.tabuleiro[i] === "") {
                    posicoesVazias.push(i);
                }
            }
            return posicoesVazias.length > 0 ? 
                posicoesVazias[Math.floor(Math.random() * posicoesVazias.length)] : 
                -1;
        }
        
        // Verifica se um jogador venceu
        function verificarVitoria(jogador) {
            var linhas = [
                [0, 1, 2], [3, 4, 5], [6, 7, 8], // linhas
                [0, 3, 6], [1, 4, 7], [2, 5, 8], // colunas
                [0, 4, 8], [2, 4, 6]             // diagonais
            ];
            
            return linhas.some(function(linha) {
                return linha.every(function(posicao) {
                    return vm.tabuleiro[posicao] === jogador;
                });
            });
        }
        
        // Verifica se o jogo terminou em empate
        function verificarEmpate() {
            return !vm.tabuleiro.includes("") && !verificarVitoria('X') && !verificarVitoria('O');
        }
        
        // Finaliza o jogo e registra o resultado
        function finalizarJogo(resultado, posicao) {
            vm.ultimaJogada = posicao;
            vm.mensagem = resultado === "Vitoria" ? "Você venceu!" : 
                          resultado === "Derrota" ? "IA venceu!" : "Empate!";
            
            // Disparar confetes se o jogador vencer
            if (resultado === "Vitoria") {
                dispararConfetes();
            }
            
            registrarJogada(resultado);
            $timeout(vm.resetarJogo, 2000);
        }

        // Função para disparar confetes
        function dispararConfetes() {
            // Lista de efeitos de confetti pré-definidos
            const efeitos = [
                // Efeito 1: Explosão clássica
                function() {
                    confetti({
                        particleCount: 150,
                        spread: 70,
                        origin: { y: 0.6 },
                        scalar: 1.2
                    });
                },
                
                // Efeito 2: Chuva de confetti
                function() {
                    confetti({
                        particleCount: 300,
                        angle: 90,
                        spread: 50,
                        startVelocity: 40,
                        origin: { y: 0 },
                        gravity: 0.5,
                        decay: 0.94
                    });
                },
                
                // Efeito 3: Explosão em leque
                function() {
                    confetti({
                        particleCount: 200,
                        angle: 60,
                        spread: 80,
                        startVelocity: 45,
                        origin: { x: 0.2, y: 0.6 }
                    });
                    confetti({
                        particleCount: 200,
                        angle: 120,
                        spread: 80,
                        startVelocity: 45,
                        origin: { x: 0.8, y: 0.6 }
                    });
                },
                
                // Efeito 4: Espirais coloridas
                function() {
                    function spiralConfetti() {
                        const count = 20;
                        const defaults = {
                            origin: { y: 0.7 },
                            ticks: 100,
                            gravity: 0.5,
                            decay: 0.94
                        };
                        
                        for (let i = 0; i < 5; i++) {
                            confetti({
                                ...defaults,
                                particleCount: count,
                                startVelocity: 20 + i * 5,
                                angle: 60 + i * 72,
                                spread: 30,
                                colors: ['#ff0000', '#00ff00', '#0000ff', '#ffff00', '#ff00ff']
                            });
                        }
                    }
                    
                    for (let i = 0; i < 3; i++) {
                        setTimeout(spiralConfetti, i * 300);
                    }
                },
                
                // Efeito 5: Fogos de artifício
                function() {
                    function firework(x) {
                        confetti({
                            particleCount: 100,
                            angle: 90,
                            spread: 70,
                            origin: { x: x, y: 0 },
                            startVelocity: 25,
                            colors: ['#ff0000', '#ffff00', '#00ffff'],
                            shapes: ['circle', 'star'],
                            scalar: 1.2
                        });
                    }
                    
                    // Dispara 3 fogos em posições diferentes
                    firework(0.25);
                    setTimeout(() => firework(0.5), 300);
                    setTimeout(() => firework(0.75), 600);
                },
                
                // Efeito 6: Confetti de formas variadas
                function() {
                    confetti({
                        particleCount: 200,
                        spread: 90,
                        origin: { y: 0.6 },
                        shapes: ['circle', 'square', 'star'],
                        colors: ['#ff0000', '#00ff00', '#0000ff', '#ffff00', '#ff00ff', '#00ffff'],
                        scalar: 1.5
                    });
                },
                
                // Efeito 7: Erupção de baixo para cima
                function() {
                    confetti({
                        particleCount: 150,
                        angle: 270,
                        spread: 50,
                        startVelocity: 50,
                        origin: { y: 1 },
                        gravity: -0.1,
                        decay: 0.9
                    });
                }
            ];
            
            // Escolhe 3 efeitos aleatórios para disparar
            const efeitosAleatorios = [];
            while (efeitosAleatorios.length < 3) {
                const randomIndex = Math.floor(Math.random() * efeitos.length);
                if (!efeitosAleatorios.includes(randomIndex)) {
                    efeitosAleatorios.push(randomIndex);
                }
            }
            
            // Dispara os efeitos com pequenos atrasos entre eles
            efeitosAleatorios.forEach((index, i) => {
                setTimeout(() => {
                    efeitos[index]();
                    
                    // Adiciona um pequeno efeito extra aleatório junto
                    if (Math.random() > 0.5) {
                        setTimeout(() => {
                            confetti({
                                particleCount: 50,
                                angle: Math.random() * 360,
                                spread: 40,
                                startVelocity: 30,
                                origin: { 
                                    x: Math.random(),
                                    y: Math.random() * 0.5 + 0.2
                                }
                            });
                        }, 200);
                    }
                }, i * 500);
            });
            
            // Efeito contínuo de fundo por 3 segundos
            const duration = 3000;
            const end = Date.now() + duration;
            const colors = ['#ff0000', '#00ff00', '#0000ff', '#ffff00', '#ff00ff'];
            
            function frame() {
                if (Date.now() > end) return;
                
                confetti({
                    particleCount: 5,
                    angle: 60,
                    spread: 55,
                    origin: { x: 0 },
                    colors: colors,
                    scalar: 0.8
                });
                
                confetti({
                    particleCount: 5,
                    angle: 120,
                    spread: 55,
                    origin: { x: 1 },
                    colors: colors,
                    scalar: 0.8
                });
                
                setTimeout(frame, 50);
            }
            
            frame();
        }        
        // Registra a jogada no backend
        function registrarJogada(resultado) {
            ApiService.postTreinar({
                EstadoTabuleiro: vm.tabuleiro.join(","),
                PosicaoEscolhida: vm.ultimaJogada,
                Resultado: resultado,
                Data: new Date().toISOString() // adiciona a data atual
            }).then(function() {
                carregarJogadas();
            }).catch(function(error) {
                console.error("Erro ao registrar jogada:", error);
            });
        }
        
        vm.formatarTabuleiro = function(estado) {
            if (!estado) return '';
            var celulas = estado.split(',');

            var html = '<div class="mini-tabuleiro">';
            for (var i = 0; i < 9; i++) {
                var valor = celulas[i] || '&nbsp;';
                var classe = valor === 'X' ? 'x' : (valor === 'O' ? 'o' : '');
                html += `<div class="mini-celula ${classe}">${valor}</div>`;
            }
            html += '</div>';
            return html;
        };
        
        // Reinicia o jogo
        vm.resetarJogo = function() {
            vm.tabuleiro = ["", "", "", "", "", "", "", "", ""];
            vm.jogadorAtual = "X";
            vm.mensagem = "";
            vm.carregando = false;
        };
        
        // Limpa todo o aprendizado da IA
        vm.limparIA = function() {
            if (confirm("Tem certeza que deseja limpar todo o aprendizado da IA?")) {
                ApiService.deleteAprendizado().then(function() {
                    vm.jogadasAprendidas = [];
                    vm.mensagem = "IA resetada com sucesso!";
                    carregarJogadas();
                }).catch(function(error) {
                    console.error("Erro ao limpar IA:", error);
                    vm.mensagem = "Erro ao resetar IA";
                });
            }
        };
        
        // Exporta os dados de aprendizado
        vm.exportarDados = function() {
            // Implementação para exportar dados
            vm.mensagem = "Dados exportados com sucesso!";
        };
        
        // Inicializa o controller
        vm.init();
    }]);