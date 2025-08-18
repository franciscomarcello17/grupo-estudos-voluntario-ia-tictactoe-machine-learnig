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
        vm.partidaId = null;
        
        // Parâmetros do Q-Learning
        vm.parametros = {
            taxaAprendizado: 0.1,
            fatorDesconto: 0.9,
            epsilon: 0.3
        };
        
        // Inicialização
        vm.init = function() {
            iniciarNovoJogo();
            carregarEstadosAprendidos();
            configurarParametros();
        };
        
        // Configura os parâmetros iniciais do Q-Learning
        function configurarParametros() {
            ApiService.definirParametros(
                vm.parametros.taxaAprendizado,
                vm.parametros.fatorDesconto,
                vm.parametros.epsilon
            ).catch(function(error) {
                console.error("Erro ao configurar parâmetros:", error);
                vm.mensagem = "Erro ao configurar parâmetros da IA";
            });
        }
        
        // Carrega os estados aprendidos pela IA
        function carregarEstadosAprendidos() {
            vm.carregandoJogadas = true;
            ApiService.getEstadosAprendidos().then(function(response) {
                vm.jogadasAprendidas = response.data || [];
            }).catch(function(error) {
                console.error("Erro ao carregar estados aprendidos:", error);
                vm.mensagem = "Erro ao carregar histórico de aprendizado";
                vm.jogadasAprendidas = [];
            }).finally(function() {
                vm.carregandoJogadas = false;
            });
        }
        
        // Inicia um novo jogo
        function iniciarNovoJogo() {
            vm.carregando = true;
            ApiService.iniciarNovoJogo().then(function(response) {
                vm.partidaId = response.data.partidaId;
                vm.tabuleiro = ["", "", "", "", "", "", "", "", ""];
                vm.celulasVencedoras = [];
                vm.ultimaJogada = null;
                vm.jogadorAtual = "X";
                vm.mensagem = "Jogo iniciado. Sua vez!";
            }).catch(function(error) {
                console.error("Erro ao iniciar jogo:", error);
                vm.mensagem = "Erro ao iniciar novo jogo";
            }).finally(function() {
                vm.carregando = false;
            });
        }
        
        // Função principal quando o jogador faz uma jogada
        vm.jogar = function(posicao) {
            if (vm.carregando || vm.tabuleiro[posicao] !== "" || vm.jogadorAtual !== "X") {
                return;
            }
            
            var jogada = {
                partidaId: vm.partidaId,
                posicao: posicao,
                simbolo: 'X'
            };
            
            vm.carregando = true;
            ApiService.fazerJogada(jogada).then(function(response) {
                atualizarEstadoJogo(response.data);
                
                // Se o jogo não terminou, é vez da IA
                if (response.data.resultado === ResultadoPartida.EmAndamento) {
                    jogadaIA();
                }
            }).catch(function(error) {
                console.error("Erro ao fazer jogada:", error);
                vm.mensagem = "Erro ao processar jogada";
                vm.carregando = false;
            });
        };
        
        // Processa a jogada da IA
        function jogadaIA() {
            vm.carregando = true;
            vm.jogadorAtual = 'O';
            
            ApiService.fazerJogadaIA(vm.partidaId).then(function(response) {
                atualizarEstadoJogo(response.data);
            }).catch(function(error) {
                console.error("Erro na jogada da IA:", error);
                vm.mensagem = "Erro na jogada da IA";
                vm.carregando = false;
                vm.jogadorAtual = 'X'; // Volta para o jogador humano em caso de erro
            });
        }
        
        // Atualiza o estado do jogo com os dados da resposta
        function atualizarEstadoJogo(jogo) {
            vm.tabuleiro = jogo.tabuleiro;
            vm.jogadorAtual = jogo.vezDoJogador === 'X' ? 'X' : 'O';
            
            if (jogo.resultado !== ResultadoPartida.EmAndamento) {
                finalizarJogo(jogo.resultado);
            } else {
                vm.mensagem = jogo.mensagem;
                vm.carregando = false;
            }
        }
        
        // Finaliza o jogo com o resultado
        function finalizarJogo(resultado) {
            switch(resultado) {
                case ResultadoPartida.VitoriaHumano:
                    vm.mensagem = "Você venceu!";
                    EfeitosFimPartida.dispararConfetes();
                    break;
                case ResultadoPartida.VitoriaIA:
                    vm.mensagem = "IA venceu!";
                    EfeitosFimPartida.chuvaDeEmojisTristes();
                    break;
                case ResultadoPartida.Empate:
                    vm.mensagem = "Empate!";
                    break;
            }
            
            $timeout(function() {
                iniciarNovoJogo();
            }, 2000);
        }
        
        // Formata o tabuleiro para exibição
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
            iniciarNovoJogo();
        };
        
        // Inicia treinamento da IA
        vm.iniciarTreinamento = function() {
            vm.carregando = true;
            ApiService.iniciarTreinamento(1000).then(function() {
                vm.mensagem = "Treinamento concluído com sucesso!";
                carregarEstadosAprendidos();
            }).catch(function(error) {
                console.error("Erro no treinamento:", error);
                vm.mensagem = "Erro durante o treinamento";
            }).finally(function() {
                vm.carregando = false;
            });
        };
        
        // Atualiza parâmetros do Q-Learning
        vm.atualizarParametros = function() {
            configurarParametros();
            vm.mensagem = "Parâmetros atualizados com sucesso!";
        };
        
        // Limpa o aprendizado da IA
        vm.limparAprendizado = function() {
            if (confirm("Tem certeza que deseja limpar todo o aprendizado da IA?")) {
                vm.carregando = true;
                ApiService.limparAprendizado().then(function() {
                    vm.mensagem = "Aprendizado da IA resetado com sucesso!";
                    carregarEstadosAprendidos();
                }).catch(function(error) {
                    console.error("Erro ao limpar aprendizado:", error);
                    vm.mensagem = "Erro ao resetar aprendizado";
                }).finally(function() {
                    vm.carregando = false;
                });
            }
        };
        
        // Inicializa o controller
        vm.init();
    }]);