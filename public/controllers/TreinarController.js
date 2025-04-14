angular.module('jogoDaVelhaApp').controller('TreinarController', 
    ['$scope', '$timeout', 'ApiService', function($scope, $timeout, ApiService) {
        var vm = this;
        
        // Estado inicial
        vm.tabuleiro = ["", "", "", "", "", "", "", "", ""];
        vm.jogadorAtual = "X";
        vm.jogadasAprendidas = [];
        vm.carregando = false;
        vm.mensagem = "";
        vm.mostrarDicas = false;
        vm.filtroTabuleiro = '';
        vm.filtroResultado = '';
        
        vm.init = function() {
            carregarJogadas();
        };
        
        function carregarJogadas() {
            vm.carregandoJogadas = true;
            ApiService.getAprendizado().then(function(response) {
                vm.jogadasAprendidas = response.data || [];
                console.log("Jogadas carregadas:", vm.jogadasAprendidas); // Para debug
            }).catch(function(error) {
                console.error("Erro ao carregar jogadas:", error);
                vm.mensagem = "Erro ao carregar histórico de jogadas";
                vm.jogadasAprendidas = [];
            }).finally(function() {
                vm.carregandoJogadas = false;
            });
        }
            
        vm.jogar = function(posicao) {
            // Validações mais simples
            if (vm.carregando || vm.tabuleiro[posicao] !== "") return;
            
            // Jogada do humano
            vm.tabuleiro[posicao] = 'X';
            
            // Verificar vitória ou empate
            if (verificarVitoria('X')) {
                registrarJogada("Vitoria");
                return;
            }
            
            if (verificarEmpate()) {
                registrarJogada("Empate");
                return;
            }
            
            // Passar vez para IA
            vm.carregando = true;
            realizarJogadaIA();
        };
        
        function realizarJogadaIA() {
            // Pequeno delay para melhor experiência
            $timeout(function() {
                ApiService.postJogarComBase(vm.tabuleiro.join(","))
                    .then(function(response) {
                        var posicaoIA = response.data.PosicaoEscolhida;
                        
                        // Fallback para jogada aleatória se necessário
                        if (posicaoIA === -1 || vm.tabuleiro[posicaoIA] !== "") {
                            posicaoIA = escolherJogadaAleatoria();
                        }
                        
                        // Aplicar jogada da IA
                        if (posicaoIA !== -1) {
                            vm.tabuleiro[posicaoIA] = 'O';
                            
                            // Verificar resultado após jogada da IA
                            if (verificarVitoria('O')) {
                                registrarJogada("Derrota");
                            } else if (verificarEmpate()) {
                                registrarJogada("Empate");
                            }
                        }
                    })
                    .catch(function(error) {
                        console.error("Erro na IA:", error);
                        // Jogada aleatória em caso de erro
                        var posicaoIA = escolherJogadaAleatoria();
                        if (posicaoIA !== -1) {
                            vm.tabuleiro[posicaoIA] = 'O';
                        }
                    })
                    .finally(function() {
                        vm.carregando = false;
                        $scope.$apply();
                    });
            }, 500);
        }
        
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
        
        function verificarEmpate() {
            return !vm.tabuleiro.includes("") && !verificarVitoria('X') && !verificarVitoria('O');
        }
        
        function registrarJogada(resultado) {
            ApiService.postTreinar({
                EstadoTabuleiro: vm.tabuleiro.join(","),
                PosicaoEscolhida: -1,
                Resultado: resultado
            }).then(function() {
                vm.mensagem = "Jogo finalizado! Resultado: " + resultado;
                carregarJogadas();
                $timeout(vm.resetarJogo, 2000);
            });
        }
        
        vm.formatarTabuleiro = function(estado) {
            if (!estado) return '';
            var celulas = estado.split(',');
            var html = '<div class="mini-tabuleiro">';
            
            for (var i = 0; i < 9; i++) {
                var valor = celulas[i] || '&nbsp;';
                var classe = celulas[i] === 'X' ? 'x' : (celulas[i] === 'O' ? 'o' : '');
                html += `<div class="mini-celula ${classe}">${valor}</div>`;
                if ((i + 1) % 3 === 0 && i < 8) html += '<br>';
            }
            
            html += '</div>';
            return html;
        };
    
        vm.resetarJogo = function() {
            vm.tabuleiro = ["", "", "", "", "", "", "", "", ""];
            vm.jogadorAtual = "X";
            vm.mensagem = "";
        };
        
        vm.limparIA = function() {
            if (confirm("Tem certeza que deseja limpar todo o aprendizado da IA?")) {
                ApiService.deleteAprendizado().then(function() {
                    vm.jogadasAprendidas = [];
                    vm.mensagem = "IA resetada com sucesso!";
                });
            }
        };
        
        vm.init();
    }]);