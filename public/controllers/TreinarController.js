angular.module('jogoDaVelhaApp').controller('TreinarController', 
    ['$scope', '$timeout', 'ApiService', 'EfeitosFimPartida', function($scope, $timeout, ApiService, EfeitosFimPartida) {
        var vm = this;
        
        // Estado inicial do jogo
        vm.tabuleiro = ["", "", "", "", "", "", "", "", ""];
        vm.celulasVencedoras = [];
        vm.ultimaJogada = null;
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
            }, 750); // Delay para melhor experiência
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
                var linhaCompleta = linha.every(function(posicao) {
                    return vm.tabuleiro[posicao] === jogador;
                });
                
                if (linhaCompleta) {
                    vm.celulasVencedoras = linha;
                    vm.jogadorVencedor = jogador;
                }
                
                return linhaCompleta;
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
                EfeitosFimPartida.dispararConfetes();
            } else if (resultado === "Derrota") {
                EfeitosFimPartida.chuvaDeEmojisTristes();
            }
            
            registrarJogada(resultado);
            $timeout(vm.resetarJogo, 2000);
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
            vm.celulasVencedoras = [];
            vm.ultimaJogada = null;
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