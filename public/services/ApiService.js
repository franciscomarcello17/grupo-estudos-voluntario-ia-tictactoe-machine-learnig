angular.module('jogoDaVelhaApp').factory('ApiService', ['$http', function($http) {
    // const baseUrl = 'https://backend-jogodavelhaia-dbbwbcfbgwhjcvgd.brazilsouth-01.azurewebsites.net/api'; // Substitua pela sua URL
    const baseUrl = 'https://localhost:7193/api'; // Substitua pela sua URL
    
    return {
        // Operações de Jogo
        iniciarNovoJogo: function() {
            return $http.post(baseUrl + '/jogo/novo');
        },
        
        fazerJogada: function(jogada) {
            return $http.post(baseUrl + '/jogo/jogar', jogada);
        },
        
        fazerJogadaIA: function(partidaId) {
            return $http.post(baseUrl + '/jogo/jogar-ia/' + partidaId);
        },
        
        obterTabuleiro: function(partidaId) {
            return $http.get(baseUrl + '/jogo/tabuleiro/' + partidaId);
        },
        
        // Operações de Treinamento
        iniciarTreinamento: function(episodios) {
            return $http.post(baseUrl + '/treinamento/iniciar?episodios=' + episodios);
        },
        
        definirParametros: function(taxaAprendizado, fatorDesconto, epsilon) {
            return $http.post(baseUrl + '/treinamento/parametros', {
                taxaAprendizado: taxaAprendizado,
                fatorDesconto: fatorDesconto,
                epsilon: epsilon
            });
        },
        
        // Operações de Aprendizado
        getEstadosAprendidos: function() {
            return $http.get(baseUrl + '/aprendizado/estados');
        },
        
        limparAprendizado: function() {
            return $http.delete(baseUrl + '/aprendizado/limpar');
        }
    };
}]);