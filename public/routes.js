angular.module('jogoDaVelhaApp').config(['$routeProvider', function($routeProvider) {
    $routeProvider
        .when('/sobre', {
            templateUrl: 'views/sobre.html',
            controller: 'MainController'
        })
        .when('/grupo', {
            templateUrl: 'views/grupo.html',
            controller: 'MainController'
        })
        .when('/treinar', {
            templateUrl: 'views/treinar.html',
            controller: 'TreinarController',
            controllerAs: 'vm'
        })
        // .when('/jogar', {
        //     templateUrl: 'views/jogar.html',
        //     controller: 'JogarController',
        //     controllerAs: 'vm'
        // })
        .when('/visualizar', {
            templateUrl: 'views/visualizar.html',
            controller: 'VisualizarController',
            controllerAs: 'vm'
        })
        .otherwise({
            redirectTo: '/treinar'
        });
}]);