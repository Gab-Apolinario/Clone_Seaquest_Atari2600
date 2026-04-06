# 🎮 Clone SeaQuest — Atari 2600 (1983)

> Clone acadêmico do clássico **SeaQuest** (Activision, 1983) desenvolvido em **Unity 6** com **C#** como projeto de prototipagem de jogos.


🕹️ **[Jogar no itch.io](https://gabriel-apolinario.itch.io/clone-seaquest-atari-2600)** | 📂 **[Código-fonte](https://github.com/Gab-Apolinario/Clone_Seaquest_Atari2600)**

---

## 📖 Sobre o Projeto

Este projeto é um clone do jogo **SeaQuest** (Activision, 1983) para Atari 2600, desenvolvido como trabalho acadêmico para a disciplina de **Prototipagem de Jogos**. A disciplina originalmente sugeria o uso da engine Construct, mas optei por utilizar a **Unity 6**, que já vinha estudando por conta própria, como forma de aprofundar meu aprendizado em uma ferramenta profissional.

O objetivo principal foi **aprender na prática** os fundamentos de desenvolvimento de jogos: arquitetura de código, sistemas de eventos, herança, física 2D, gerenciamento de estados, UI, áudio e publicação WebGL — tudo construído do zero.

---

## 🎯 O Jogo Original

No SeaQuest original, o jogador controla um submarino que deve resgatar mergulhadores no fundo do mar enquanto desvia e atira em tubarões e submarinos inimigos. A barra de oxigênio diminui constantemente enquanto submerso, forçando o jogador a retornar à superfície periodicamente. A cada rodada bem-sucedida (subir com 6 mergulhadores), a dificuldade aumenta.

---

## 🕹️ Como Jogar

| Ação | Controle |
|---|---|
| Mover | `WASD` ou `Setas` |
| Atirar | `Espaço` |
| Reiniciar | `R` |

### Regras Principais

- **Oxigênio**: Diminui enquanto submerso. Se zerar, você morre.
- **Mergulhadores**: Colete até 6 e suba à superfície para ganhar pontos.
- **Subir sem mergulhadores**: Perde 1 vida.
- **Subir com menos de 6**: Perde 1 mergulhador coletado.
- **Rodada de sucesso** (6 mergulhadores): Pontos bônus pelo oxigênio restante + pontos por mergulhador. A dificuldade aumenta.
- **Vida extra**: A cada 10.000 pontos.
- **Pontuação escalonada**: Inimigos começam valendo 20 pts (máx. 90), mergulhadores começam valendo 50 pts (máx. 1.000), ambos aumentando a cada rodada de sucesso.

---

## 🏗️ Arquitetura do Projeto

### Estrutura de Pastas

```
Assets/
├── PREFABS/
│   ├── Peixe
│   ├── Submarino
│   ├── Humano
│   ├── Player
│   └── TiroJogador
├── SCRIPTS/
│   ├── MANAGERS/
│   │   ├── GameManager.cs      — Máquina de estados, pontuação, vidas, oxigênio
│   │   ├── Acoes.cs            — Central de eventos (Actions estáticas)
│   │   ├── UIManager.cs        — Atualização de UI via eventos
│   │   └── AudioManager.cs     — Efeitos sonoros via eventos
│   ├── FabricaInimigos/
│   │   ├── BaseInimigo.cs      — Classe base (herança)
│   │   └── FabricaInimigos.cs  — Fábrica/spawner de inimigos
│   ├── PreFabs/
│   │   ├── Peixe.cs            — Herda de BaseInimigo
│   │   ├── Submarino.cs        — Herda de BaseInimigo (atira)
│   │   ├── Humano.cs           — Herda de BaseInimigo (coletável)
│   │   ├── SubPatrulheiro.cs   — Submarino patrulheiro da superfície
│   │   └── SpawnerSubPatrulheiro.cs — Spawner dedicado do patrulheiro
│   ├── Player.cs               — Movimento, tiro, input
│   ├── TiroJogador.cs          — Projétil do jogador
│   └── TiroSubmarino.cs        — Projétil do submarino inimigo
└── SCENES/
    └── SampleScene
```

### Sistema de Eventos — `Acoes.cs`

O projeto utiliza um sistema de **eventos desacoplados** com `Action` delegates do C#, centralizados em uma classe estática `Acoes.cs`. Isso permite que scripts se comuniquem sem referências diretas entre si.

**Fluxo**: Declarar (`Acoes.cs`) → Transmitir (`?.Invoke()`) → Escutar (`+= / -=` em `OnEnable/OnDisable`)

**Eventos implementados:**

| Evento | Tipo | Transmissor | Ouvinte(s) |
|---|---|---|---|
| `JogadorMorto` | `Action<int>` | BaseInimigo, Player, SubPatrulheiro | GameManager, AudioManager |
| `InimigoMorto` | `Action<int>` | BaseInimigo | GameManager |
| `ColetouHumano` | `Action<int>` | Humano | GameManager, AudioManager |
| `MoverJogador` | `Action<bool, Vector2>` | GameManager | Player |
| `Superficie` | `Action<bool>` | GameManager | Player, SubPatrulheiro |
| `AtivarSpawn` | `Action<bool>` | GameManager | FabricaInimigos |
| `PeixeMorto` | `Action` | BaseInimigo | AudioManager |
| `SubmarinoMorto` | `Action` | BaseInimigo | AudioManager |
| `TiroJogador` | `Action` | Player | AudioManager |
| + eventos de UI | diversos | GameManager | UIManager |

### Herança — Sistema de Inimigos

```
MonoBehaviour
  └── BaseInimigo (movimento, colisão, destruição, pontos)
        ├── Peixe (TipoPeixe enum, cores/velocidades por dificuldade)
        ├── Submarino (sistema de tiro com coroutine)
        └── Humano (coleta pelo jogador, ignora tiros)
```

A `FabricaInimigos` instancia os prefabs e configura a direção via `IrDireita()` — padrão **fábrica-configura-produto** (a fábrica determina o comportamento, não o produto).

### Máquina de Estados — `GameManager.cs`

O jogo opera em três estados gerenciados pelo enum `EstadoJogo`:

- **Superficie**: Oxigênio enchendo, jogador travado até encher, spawn de patrulheiro
- **Submerso**: Oxigênio diminuindo, spawn de inimigos ativo, jogador pode atirar
- **GameOver**: Tela congelada (`Time.timeScale = 0`), prefabs destruídos

A transição é centralizada em `MudarEstadoJogo()` com um `switch/case`.

### Sistema de Dificuldade

A dificuldade escala por **rodadas de sucesso** (não por tempo):
- `multiplicadorDificuldade` aumenta +0.1 por rodada
- Velocidade dos inimigos é multiplicada pelo fator
- Intervalo de spawn diminui (mín. 0.2s)
- Novos tipos de peixe são desbloqueados (Dificuldade2 a partir de 1.1, Dificuldade3 a partir de 1.3)
- Submarinos inimigos começam a spawnar a partir de 1.1
- Submarino patrulheiro aparece a partir de 1.2

---

## 🧠 O Que Aprendi

### Programação & Arquitetura
- **Sistema de eventos com C# Actions**: delegates estáticos, `?.Invoke()`, subscribe/unsubscribe em `OnEnable`/`OnDisable`
- **Herança em C#**: `virtual`/`override`/`protected`/`base`, classes base com comportamento compartilhado
- **Máquina de estados**: enum + switch/case centralizado para controle de fluxo do jogo
- **Coroutines**: delays, loops com `yield return`, guards para evitar chamadas a cada frame
- **Padrão fábrica**: spawner configura o produto pós-instanciação

### Unity
- **New Input System**: Action Maps, bindings, classe C# gerada (`InputSystem_Actions`)
- **Rigidbody2D + MovePosition**: movimento baseado em física com clamping de limites
- **Trigger vs Collision**: diferença conceitual e prática (apenas um objeto precisa de Rigidbody2D para triggers)
- **WebGL Build**: remoção de `UnityEditor` namespaces, configuração de canvas 640×480

### Conceitos Gerais
- **Variáveis estáticas**: `jogadorCheio` como estado global legível por qualquer instância
- **Sprite color tinting**: base branca para multiplicação limpa com `SpriteRenderer.color`
- **Git/GitHub**: versionamento com `.gitignore` para Unity, commits no infinitivo, `--amend` + force push

---

## 🛠️ Ferramentas & Tecnologias

| Ferramenta | Uso |
|---|---|
| **Unity 6** | Engine de desenvolvimento |
| **C#** | Linguagem de programação |
| **Unity New Input System** | Sistema de input |
| **Git / GitHub** | Controle de versão |
| **itch.io** | Publicação WebGL |

### Assets

- **Sprites**: Prints do jogo original (requisito da atividade acadêmica, não era para focar na criação da arte)
- **Áudio**: Efeitos sonoros em MP3 extraídos do projeto [SeaQuestJs](https://github.com/kortkamp/SeaQuestJs) (sons originais do jogo de 1983)
- **Referência**: [Manual original do SeaQuest (1983)]

---

## 📋 Fases de Desenvolvimento

1. **Arquitetura & Planejamento** — Estudo do jogo original, definição de entidades e diagramas de arquitetura
2. **Setup do Projeto** — Unity 6, geração de sprites
3. **Movimentação do Jogador** — New Input System, Rigidbody2D, clamping, sprite flip
4. **Sistema de Tiro** — Instanciação de projéteis, cooldown via coroutine, direção baseada no sprite
5. **Fábrica de Inimigos** — Spawner com probabilidade ponderada, configuração de direção, tick escalável
6. **Herança de Inimigos** — BaseInimigo → Peixe (tipos/cores), Submarino (tiro), Humano (coleta)
7. **Sistema de Eventos** — `Acoes.cs` centralizado, colisões disparando eventos, múltiplos ouvintes
8. **GameManager & Estados** — Máquina de estados, pontuação escalonada, mecânica de oxigênio, vidas extras
9. **UI & Feedback Visual** — UIManager via eventos, barra de oxigênio piscante, ícones de mergulhadores
10. **Áudio** — AudioManager com enum, PlayOneShot para concorrência, loop para alerta de oxigênio
11. **Dificuldade & Patrulheiro** — Escalonamento progressivo, SubPatrulheiro com spawner dedicado
12. **Publicação** — Build WebGL, correção de aspect ratio 4:3, deploy no itch.io

---

## 🎓 Contexto Acadêmico

Este projeto foi desenvolvido como trabalho para a disciplina de **Prototipagem de Jogos** de um curso de graduação. A disciplina sugeria o uso do Construct, mas escolhi a Unity 6 para aprofundar meu aprendizado em uma engine profissional que já vinha estudando por conta própria. O projeto é **não-comercial** e tem fins exclusivamente educacionais.

---

## 📄 Licença

Projeto acadêmico sem fins comerciais. O jogo original SeaQuest é propriedade da Activision (1983).
