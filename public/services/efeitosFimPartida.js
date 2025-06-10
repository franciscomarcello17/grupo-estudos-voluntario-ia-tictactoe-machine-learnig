angular.module('jogoDaVelhaApp').factory('EfeitosFimPartida', function($timeout) {
    return {
        dispararConfetes: function() {
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
        },
        
        chuvaDeEmojisTristes: function() {
            const emojis = ["😞", "😔", "😢", "😭", "☹️"];
            const quantidade = 100;
            const duracaoAnimacao = 3000; // duração da queda em ms (5 segundos)

            for (let i = 0; i < quantidade; i++) {
                setTimeout(() => {
                    const emoji = document.createElement('div');
                    emoji.textContent = emojis[Math.floor(Math.random() * emojis.length)];
                    emoji.style.position = 'fixed';
                    emoji.style.left = Math.random() * 100 + 'vw';  // espalha em toda a largura da tela
                    emoji.style.top = '-2em';
                    emoji.style.fontSize = `${Math.random() * 24 + 24}px`;
                    emoji.style.opacity = Math.random() * 0.5 + 0.5; // entre 0.5 e 1
                    emoji.style.zIndex = 9999;
                    emoji.style.pointerEvents = 'none';
                    emoji.style.transition = `transform ${duracaoAnimacao}ms ease-in`;

                    document.body.appendChild(emoji);

                    // Força o reflow antes de aplicar a animação
                    void emoji.offsetWidth;

                    emoji.style.transform = `translateY(${window.innerHeight + 100}px) rotate(${Math.random() * 360}deg)`;

                    // Remove o emoji após a animação
                    setTimeout(() => {
                        emoji.remove();
                    }, duracaoAnimacao);
                }, i * 30);  // espaça a criação de cada emoji em 150ms
            }
        }
    };
});