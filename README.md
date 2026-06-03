# 🩸 ATTACK ON TITAN: CRIMSON CORPUS 🩸

## PILARES DE DESIGN

```
┌─────────────────────────────────────────────────────────────────┐
│                                                                 │
│   ██  TONALIDADE SOMBRIA                                        │
│   ██  Desespero, horror existencial, vulnerabilidade humana     │
│                                                                 │
│   ██  MECÂNICAS DE ESCASSEZ                                     │
│   ██  Cada recurso é precioso; cada decisão tem peso mortal     │
│                                                                 │
│   ██  PALETA MONOCROMÁTICA VERMELHA                             │
│   ██  Sangue, carne, vapor e fogo — o vermelho é a única cor    │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## 1. VISÃO GERAL

| Item | Descrição |
|------|-----------|
| **Título** | Attack on Titan: Crimson Corpus |
| **Gênero** | Metroidvania Vertical / Survival Horror Exploratório |
| **Tom** | **Sombrio, opressivo, desesperançoso** — inspirado em Limbo, Inside e Blasphemous |
| **Estética** | **Monocromática vermelha** — todo o mundo é renderizado em tons de vermelho, do negro-sangue ao rosa-vapor |
| **Mecânica Central** | **Escassez como linguagem** — gás, lâminas e vida do personagem são recursos finitos que forçam a tomada de decisões |
| **Perspectiva** | 2D Side-Scrolling vertical |
| **Orientação** | Torre/Muralhas |

---

## 2. DIREÇÃO DE ARTE — PALETA MONOCROMÁTICA VERMELHA

### 2.1 Filosofia Visual

O jogo inteiro existe dentro do espectro do vermelho. Não há azul. Não há verde. Não há esperança em outras cores. O vermelho é o sangue dos soldados, o vapor dos Titãs, o fogo de Shiganshina, a carne exposta, o pôr-do-sol que nunca termina. O mundo parece preso num crepúsculo eterno — ou dentro de uma ferida.

A ausência de outras cores comunica:
- **Desumanização** — o mundo perdeu sua diversidade, sua vida
- **Obsessão com a morte** — tudo lembra sangue e carne
- **Claustrofobia cromática** — o jogador não tem "descanso visual"
- **Identidade inconfundível** — reconhecível em uma única screenshot

### 2.2 Uso Narrativo da Cor

- **Quanto mais alto o jogador sobe (Wall Sina), mais claro e "lavado" o vermelho** — a riqueza dos nobres é uma palidez doentia, quase rosa. A abundância é retratada como algo anêmico, sem vida real.
- **Quanto mais fundo/baixo, mais escuro e saturado** — o sangue seca nas ruínas exteriores, o vermelho se torna quase negro. A morte é densa.
- **Os Titãs são sempre mais saturados que o ambiente** — eles são a coisa mais "viva" (e mais horrível) do cenário. Seus corpos pulsam em carmesim.

---

## 3. SISTEMA DE ESCASSEZ 

### 3.1 Filosofia da Escassez

A escassez não é um sistema paralelo — ela **É** a experiência. Cada recurso no jogo é finito e precioso. O jogador nunca se sente confortável. A abundância não existe. Quando o gás acaba no meio de uma escalada, o jogador cai. Quando as lâminas quebram contra a nuca de um Titã, sobra o desespero. Essa tensão constante transforma cada decisão em uma mini-narrativa de sobrevivência.

### 3.2 Recursos Escassos — Tabela Completa

| Recurso | Função | Como Obtém | Como Perde | Quando Acaba... |
|---|---|---|---|---|
| **Gás ODM** | Combustível para TODOS os movimentos aéreos (gancho, dash, swing) | Cilindros encontrados em cadáveres de soldados, quartéis abandonados, drops raros | Cada uso do gancho/dash consome gás; uso sob chuva consome mais | Sem gás = sem ODM. O jogador fica **preso ao chão**, incapaz de subir, vulnerável. Deve caminhar até achar um cilindro ou morrer. |
| **Lâminas** | Arma principal contra Titãs (cortar a nuca) | Encontradas em cadáveres, forjas raras, armários militares | Cada golpe reduz durabilidade; golpes em armadura/cristal gastam mais; lâminas podem **quebrar no meio de um ataque** | Sem lâminas = sem dano a Titãs. O jogador só pode fugir ou se esconder. |
| **Sangue (HP)** | Vida do personagem | Bandagens (raras), kit médico (extremamente raros), fogueiras (cura parcial, não total) | Ataques de Titãs, quedas, armadilhas, exposição ao vapor | Morte. Retorno ao último save. Recursos obtidos voltam para seus pontos de origem. |
| **Integridade Física** | Estado do corpo — afeta performance | Não se "ganha"; apenas se perde menos | Quedas, ataques pesados, uso excessivo do ODM em braços feridos | Braço ferido = gancho mais lento. Perna ferida = salto menor. |
| **Sinalizadores** | Acendem fogueiras apagadas; iluminam áreas escuras; servem de distração | Encontrados em suprimentos militares (2-3 por zona) | Cada uso é permanente — uma vez disparado, não volta | Sem sinalizadores = sem checkpoints. |

### 3.3 Regras Sistêmicas da Escassez

**1. Nada é de graça.**
- Fogueiras curam HP parcialmente, não totalmente. Bandagens curam HP, mas não curam feridas. A cura total exige kit médico (item consumível raro).
- Salvar o jogo em uma fogueira **NÃO** recupera gás ou lâminas.
- **Exceção de misericórdia:** Se o jogador morrer 3x no mesmo ponto sem gás/lâminas, um "corpo de soldado" aparece próximo ao save com suprimentos mínimos de emergência. Isso evita softlock, mas a quantidade é mísera.

**3. Combater é caro. Fugir é uma opção real.**
- Nem todo Titã precisa ser morto. Muitos encontros são projetados para fuga/stealth.
- Matar um Titã normal gasta 1-2 lâminas e gás considerável. O retorno? Às vezes nenhum drop.
- Titãs Anormais são erráticos — o risco de combater é altíssimo.
- O jogador aprende a **evitar** combate quando possível, reservando recursos para bosses e áreas obrigatórias.

**4. Inventário limitado.**
- O jogador carrega no máximo: **4 pares de lâminas, 2 cilindros de gás reserva, 3 bandagens, 2 sinalizadores, 1 kit médico**.
- Slots são fixos. Não há expansão de inventário. Isso é intencional — humanos não são containers.
- Trocar itens no chão é possível, mas o item largado fica naquele ponto do mapa (e pode ser perdido se uma área colapsar, nunca se sabe >:]).

**5. O ambiente também consome.**
- Vapor de Titã reduz visibilidade e pode causar dano.
- Se molhar aumenta consumo de gás (mecanismo úmido).

### 3.4 O Ciclo de Tensão da Escassez

```
  ┌──────────────────────────────────────────┐
  │                                          │
  │    EXPLORAR                              │
  │    (gasta gás, arrisca lâminas)          │
  │         │                                │
  │         ▼                                │
  │    ENCONTRAR RECURSO?                    │
  │    ┌─── SIM ──→ Alívio breve             │
  │    │              │                      │
  │    │              ▼                      │
  │    │         DECISÃO:                    │
  │    │         Usar agora ou guardar?      │
  │    │              │                      │
  │    NÃO            ▼                      │
  │    │         Continuar explorando        │
  │    │              │                      │
  │    ▼              │                      │
  │  TENSÃO CRESCE ◄──┘                      │
  │    │                                     │
  │    ▼                                     │
  │  ENCONTRAR TITÃ                          │
  │    │                                     │
  │    ├── LUTAR (gasta recursos)            │
  │    │     └── Vitória? Drop mísero        │
  │    │     └── Derrota? Perde tudo         │
  │    │                                     │
  │    └── FUGIR (gasta gás, ganha nada)     │
  │           └── Mas sobrevive              │
  │                                          │
  │  ──→ Repetir até a próxima fogueira ──→  │
  │                                          │
  └──────────────────────────────────────────┘
```

---

## 4. CARACTERÍSTICAS METROIDVANIA × ATTACK ON TITAN

### 4.1 MAPA INTERCONECTADO (MUNDO ÚNICO)

**Aplicação do Metroidvania:**
Um mundo único, contíguo. A torre é uma mega-estrutura vertical. A estrutura concêntrica das três muralhas é "achatada" num corte vertical:

| Zona(s) | Tema AoT | Tons Dominantes | Atmosfera Sombria |
|---|---|---|---|
| Zona 0 | Esgotos / Subterrâneo | Vermelho escuro / Carmesim | Túneis de tijolos/pedregulhos ensanguentados. Água turva vermelha. Sons de algo espreitando, mas é incerto do que pode ser |
| Zona 1 | Base da torre / Salas de solados | Marrom Sangue / Carne Escura | A base da torre está em ruínas. Manchas de sangue velho nas paredes. Fogueiras apagadas. Cadaveres de soldados com equipamentos saqueável |
| Zona 2 | Parte alta da torre / Nobreza | Vermelho Claro / Escarlate | Tudo é "limpo" e "bonito", mas a palidez é de causar nausea. Os nobres sorriem com olhos vazios. |
| Zona 3 | Caminho de Ymir | Abaixo dos Esgotos | Cristais Vermelhos / Veias escarlates | A zona final é escuridão com veias de luz vermelha. O vapor aqui é tóxico. Aqui é onde a realidade se desfaz |

**Conexões:**
- Verticais: Escalar a muralha com ODM, elevadores primitivos.
- Horizontais: Túneis dentro da muralha, esgotos, passagens de contrabando.
- Secretas: Passagens dentro da própria muralha.

**Relação com a Escassez:** O mapa é interconectado, mas percorrer conexões custa recursos. Um atalho vertical economiza tempo mas gasta gás. Um túnel horizontal é seguro mas longo. Cada rota tem um custo. Não existe caminho "grátis".

**Relação com o Tom Sombrio:** Não há zonas "felizes" ou "seguras". Cada bioma é uma variação de horror — da desolação das ruínas à opressão doentia do luxo de Sina. O alívio nunca chega.

### 4.2 BARREIRAS DE HABILIDADE (ABILITY GATING)

**Aplicação do Metroidvania:**
Obstáculos que só são superados com habilidades específicas. Controlam o fluxo de progressão e dão propósito a cada power-up. As barreiras refletem a brutalidade do mundo. Não são "portas mágicas" — são obstáculos físicos, militares e biológicos:

| Barreira | Habilidade Necessária | Referência AoT | Custo da Escassez |
|---|---|---|---|
| Paredes sem apoio | **Ganchos ODM** (grapple) | Equipamento de manobra vertical | Cada uso do gancho consome gás |
| Abismos entre muralhas | **Propulsão a Gás** (dash aéreo) | Cilindros de gás | Dash consome 3x mais gás que gancho |
| Blocos de cristal/endurecimento/portões de aço | **Estocada da Armaguarda** | Lâminas | Gasta 1 lâmina por uso; feridas no braço se utilizado 2x seguidas |
| Zonas de vapor denso | **Máscara de Gás** (item permanente) | Exploração do Caminho de Ymir | Estar no vapor sem máscara drena HP |

### 4.3 BACKTRACKING (REVISITAÇÃO DE ÁREAS)

**Aplicação do Metroidvania:**
O backtracking não é só mecânico — é **emocionalmente pesado**. Voltar a um lugar é reencontrar os mortos que você deixou para trás.

Por exemplo: 
- **Retorno a Zona 0:** Com Estocada da Armaguarda obtida na Zona 1, o jogador pode destruir areas cristalizadas ou portões que bloquavam a passagens.
- **Retorno a Zona 3:** Com a máscara de gás obtida na Zona 2, o jogador pode acessar o Abismo de Ymir

### 4.4 ATMOSFERA, NARRATIVA AMBIENTAL E TRILHA SONORA

**Aplicaçãode Metroidvania:**
Narrativa contada através do ambiente e level design. A atmosfera (visual + sonora) é fundamental para a imersão. 
**Narrativa ambiental — o mundo fala através dos mortos:**
- Corpos de soldados em posições que contam histórias: abraçados, fugindo, lutando, rendidos.
- Marcas de lâminas ODM nas paredes = rotas de fuga. Quanto mais marcas, mais desespero houve ali.
- Escritos nas paredes em Sangue Fresco: mensagens de soldados enlouquecidos.


**Trilha sonora — O silêncio é a norma, o som é o horror:**
- 80% da exploração é em **silêncio** — apenas sons ambientes: vento, ranger de metal, gotas, ecos distantes.
- Quando um Titã se aproxima: **batidas cardíacas** na trilha. Lentas. Ficam mais rápidas.
- Combate com bosses: coral dissonante e percussão tribal (inspirado em Sawano mas distorcido, como se a música estivesse "quebrando").
- Nas fogueiras: um tema melancólico solo de violoncelo. Breve. Frágil. Pode ser interrompido a qualquer momento por um rugido ao longe.

**Relação com a Paleta:** O som e a cor trabalham juntos — zonas mais escuras são mais silenciosas. Zonas mais claras têm um zumbido constante, quase orgânico, como se as paredes respirassem.

### 4.5 BOSSES E MINI-BOSSES

**Aplicação de Metroidvania:**
Chefes que guardam habilidades ou passagens críticas. Testam habilidades recém-adquiridas e oferecem desafios memoráveis. Cada boss concede um novo poder.

Cada boss é um **evento traumático**. Vencer não traz alegria — traz alívio e custo.

| Boss | Zona | Poder Concedido | Mecânica de Luta |
|---|---|---|---|
| **Titã Blindado** | 0→1 | Estocada da Armaguarda | Encontrar brechas na armadura usando gás para flanquear. Ataques frontais são inúteis. 
| **Titã Bestial** | 1→2 | Arremesso (ranged attack) | Ele fica de olho nas janelas e arremessa escombros qunado o jogador por uma janela. O jogador precisa subir sob fogo cruzado. 
| **Titã Colossal** | 2 ("exterior") | Gas Boost (dash) | Escalar o corpo enquanto vapor drena HP. Ele se move devagar mas cada passo é um grande ataque |
| **Titã Fundador** | 3 | Coordenada (final) | Multi-fase dentro do Caminho. A realidade se distorce. As fases replicam zonas anteriores, mas corrompidas. A luta final não tem trilha sonora. Só silêncio e os sons do jogador. É a coisa mais aterrorizante do jogo. |

**Mini-bosses — Titãs Anormais:**
- Aparecem aleatoriamente em qualquer zona.
- Comportamento imprevisível: correm, pulam e vão direto no jogador.
- Não concedem poder. Drop é aleatório.
- Existem para lembrar que o mundo é injusto.

### 4.6 SISTEMA DE MAPA E ORIENTAÇÃO

**Aplicação de Metroidvania:**
Mapa que se revela com a exploração:

- **O mapa começa totalmente em negro.** O mapa parece uma mancha de sangue se espalhando.
- **Sem mapa automático.** O jogador precisa encontrar "Anotações de Scouts" para revelar porções do mapa. 
- **Sem indicadores de objetivos.** O mapa mostra onde você esteve. Não mostra para onde ir. A desorientação é intencional.

**Relação com a Escassez:** O próprio mapa é escasso. Informação é recurso. O jogador que desperdiça sinalizadores fica sem marcadores. O jogador que não encontra anotações de scouts navega às cegas.

### 4.7 SAVE POINTS / PONTOS DE DESCANSO

**Aplicação de Metroidvania:**
**Fogueiras dos Scouts** são pontos de save e recuperação espalhados pelo mapa. Âncoras seguras na exploração.

| Função | Disponibilidade | Limitação |
|---|---|---|
| Salvar | Sempre | — |
| Recuperar HP | Sempre | Cura apenas **50% do HP máximo**. Cura total exige ração (consumível raro). |
| Recuperar Sanidade | Sempre | Recupera apenas **30%**. Ter aliados vivos na fogueira recupera mais. |
| Reabastecer gás/lâminas | **NUNCA** | Gás e lâminas NÃO são recuperados em fogueiras. Devem ser encontrados. |
| Conversar com NPCs | Se o NPC estiver vivo | NPCs podem morrer permanentemente. Fogueiras ficam vazias. |
| Fast travel | Só entre fogueiras com elevador funcional | Elevadores requerem combustível (gás). |
| Recuperar Vontade Titã | Lentamente | 25% por descanso. Descansar múltiplas vezes não é possível — o fogo se apaga. |

**Frequência:** 2-3 fogueiras por zona. A Zona 3 tem **uma única fogueira** no início e nenhuma depois.
**Visual:** A fogueira é a única fonte de luz "quente"

### 4.8 FAST TRAVEL (VIAGEM RÁPIDA)

**Aplicação de Metroidvania:**
Sistema de viagem rápida entre pontos já visitados para reduzir tedium no backtracking. É feito por meio de elevadores e sistemas de esgoto.
