
# Проект "Take an apple"

<div align="center">
  <img width="690" height="390" alt="MatchReview" src="https://github.com/user-attachments/assets/500e49ed-3564-4c2d-9ff6-95e7937a4c9c" />
</div>

## Описание проекта

### Использованные паттерны и технологии:
- **Reflex DI** — dependency injection через `ContainerBuilder` и `BindingModule`
- **ScriptableObject** — `AudioBank`, `ItemSlot` для data-driven конфигурации
- **Observer Pattern** — события через `Action`, `event` для decoupled-архитектуры
- **Interface Segregation** — `IAudioService`, `IAdvertising`, `IGameEvents`
- **Component-based architecture** — модульные `MonoBehaviour` с чёткими зонами ответственности
- **MVC/MVVM** — разделение `LevelsController` (logic) и `LevelsView` (presentation)
- **Disposable Pattern** — гарантированная очистка подписок и ресурсов
- **Strategy Pattern** — переключаемая реализация рекламы (Dev/Production)

### Core Gameplay — Flask Sequence:
- Система слотов для предметов (`ItemSlot` с сериализацией)
- Логика заполнения колбы: проверка совпадения типов предметов
- Событийная модель: `event Action OnFilled` для реакции на завершение комбинации
- Методы `TryAddItem`, `GetFirstItem`, `PeekFirstItem` для управления инвентарём колбы
- LINQ-запросы для поиска свободных слотов и валидации состояния

### Визуальные и аудио-эффекты:
- **VFX**: `ParticleSystem` для визуализации заполнения колбы
- **SFX**: `AudioEvent` + `IAudioService` для проигрывания звуков
- Data-driven аудио-конфигурация через `AudioBank` (ScriptableObject)
- Позициональный звук для иммерсивности (`PlayOneShot(position)`)

### Система уровней:
- `LevelsController` с разделением логики и представления (MVC-подход)
- Состояния уровней: `Opened` / `Locked` / `Completed`
- Волновая загрузка и перезапуск уровней через `LevelCreator`
- Событийная коммуникация: `OnLevelButtonClicked`, `OnLevelStateChanged`

### Монетизация:
- Интерфейс `IAdvertising` с реализацией для dev-режима (`DevAdvertising`)
- Interstitial и Rewarded реклама с cooldown-таймером
- Коллбэк-система: `Action onSuccess/onError` для гибкой интеграции

### Оптимизация:
- Object Pooling потенциально для частиц и аудио-источников
- Корректная подписка/отписка от событий через `IDisposable`
- Кэширование компонентов (`GetComponent` в `Awake`)
- LINQ с осторожностью для UI-логики (не в Update)





