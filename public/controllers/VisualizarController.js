angular.module('jogoDaVelhaApp').controller('VisualizarController', 
    ['$interval', 'ApiService', function($interval, ApiService) {
        var vm = this;
        
        vm.jogadas = [];
        vm.carregando = true;
        vm.itensPorPagina = 10;
        vm.opcoesPorPagina = [10, 20, 50, 100, 'Todos'];
        vm.paginaAtual = 1;
        vm.searchText = '';

        vm.init = function() {
            carregarJogadas();
            // Atualiza a cada 60 segundos (se deixar menos, pode exceder limites do banco de dados)
            vm.intervalPromise = $interval(carregarJogadas, 60000);
        };
        
        vm.eMobile = window.innerWidth <= 768;

        window.addEventListener('resize', function() {
            vm.eMobile = window.innerWidth <= 768;
        });

        function carregarJogadas() {
            ApiService.getAprendizado().then(function(response) {
                vm.jogadas = response.data.sort(function(a, b) {
                    return new Date(b.data) - new Date(a.data);
                });
                vm.carregando = false;
            }).catch(function(error) {
                console.error("Erro ao carregar jogadas:", error);
                vm.carregando = false;
            });
        }

vm.getJogadasPaginadas = function() {
    var jogadasFiltradas = vm.getJogadasFiltradas();
    if (vm.itensPorPagina === 'Todos') return jogadasFiltradas;
    
    var inicio = (vm.paginaAtual - 1) * vm.itensPorPagina;
    var fim = inicio + vm.itensPorPagina;
    return jogadasFiltradas.slice(inicio, fim);
};

vm.getJogadasFiltradas = function() {
    if (!vm.searchText) return vm.jogadas;
    return vm.jogadas.filter(function(jogada) {
        var texto = JSON.stringify(jogada).toLowerCase();
        return texto.includes(vm.searchText.toLowerCase());
    });
};

vm.paginasVisiveis = function() {
    var total = vm.totalPaginas();
    var paginaAtual = vm.paginaAtual;
    var maxPaginas = vm.eMobile ? 1 : 10;
    var paginas = [];

    if (total <= maxPaginas) {
        for (var i = 1; i <= total; i++) {
            paginas.push(i);
        }
    } else {
        var inicio = Math.max(1, paginaAtual - Math.floor(maxPaginas / 2));
        var fim = inicio + maxPaginas - 1;

        if (fim > total) {
            fim = total;
            inicio = fim - maxPaginas + 1;
        }

        for (var i = inicio; i <= fim; i++) {
            paginas.push(i);
        }

        if (inicio > 2) {
            paginas.unshift('...');
            paginas.unshift(1);
        } else if (inicio === 2) {
            paginas.unshift(1);
        }

        if (fim < total - 1) {
            paginas.push('...');
            paginas.push(total);
        } else if (fim === total - 1) {
            paginas.push(total);
        }
    }

    return paginas;
};

vm.totalPaginas = function() {
    if (vm.itensPorPagina === 'Todos') return 1;
    return Math.ceil(vm.getJogadasFiltradas().length / vm.itensPorPagina);
};

        vm.irParaPagina = function(pagina) {
            if (pagina >= 1 && pagina <= vm.totalPaginas()) {
                vm.paginaAtual = pagina;
            }
        };

        vm.proximaPagina = function() {
            if (vm.paginaAtual < vm.totalPaginas()) {
                vm.paginaAtual++;
            }
        };

        vm.paginaAnterior = function() {
            if (vm.paginaAtual > 1) {
                vm.paginaAtual--;
            }
        };

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
        
        // Limpa o interval quando o controller é destruído
        vm.$onDestroy = function() {
            if (vm.intervalPromise) {
                $interval.cancel(vm.intervalPromise);
            }
        };
        
        vm.init();
    }]);