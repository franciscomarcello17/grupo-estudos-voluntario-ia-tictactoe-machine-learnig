angular.module('jogoDaVelhaApp').controller('SobreController', 
    ['$scope', function($scope) {
        var vm = this;
        
        // Inicializa a seção ativa
        vm.activeSection = 'idea';
        
        // Função para mudar de seção
        vm.setActiveSection = function(section) {
            vm.activeSection = section;
        };
        
        // Verifica se uma seção está ativa
        vm.isActiveSection = function(section) {
            return vm.activeSection === section;
        };

        // No SobreController
        vm.openProfile = function(member) {
            const profiles = {
                'torrens': 'https://buscatextual.cnpq.br/buscatextual/visualizacv.do?metodo=apresentar&id=K4732940Z2    ',
                'dumba': 'https://buscatextual.cnpq.br/buscatextual/visualizacv.do?metodo=apresentar&id=K9107865P8',
                'lima': 'https://br.linkedin.com/in/francisco-marcello-ribeiro-lima-878247265'
            };
            
            if (profiles[member]) {
                window.open(profiles[member], '_blank');
            }
        };
    }]);