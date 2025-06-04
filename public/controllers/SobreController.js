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
    }]);