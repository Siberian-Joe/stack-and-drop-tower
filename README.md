# stack-and-drop-tower

Мини-игра с нижним скроллом кубиков, построением башни справа и дырой слева.
Фокус: масштабируемость логики, чистая архитектура, сохранение прогресса и локализация.

---

## Быстрый старт

1. Открой сцену **Bootstrap**.
2. Нажми **Play**.

В `ProjectContext` на `AppInstaller` должны быть назначены ссылки:

* `GameConfig` (Addressables `AssetReferenceT<GameConfigAsset>`)
* `SceneCatalog`
* `ScreenRootsPrefab`
* `ScreenCatalog`
* `LocalizationJson` (Addressables `AssetReferenceT<TextAsset>`)

### Как тестово переключить язык

Для быстрой проверки локализации достаточно заменить `LocalizationJson` в `ProjectContext` на `AppInstaller` на другой Addressables `TextAsset` (JSON) с нужным языком.

После старта приложение:

* поднимет `ScreenRoots`,
* покажет `LoadingOverlay`,
* загрузит конфиг / локализацию / прогресс,
* переключится на сцену **Core**.

---

## Игровые правила (как задумано)

### Нижняя панель (Bottom bar)

* Горизонтальный `ScrollRect` с **N** кубиками.
* Кубики **бесконечные**: при начале перетаскивания берётся **клон**, оригинал остаётся в скролле.

### Башня (справа)

* Первый кубик можно поставить в область башни (определяется `TowerRoot` + правило `PointerInsideTowerDropAreaRule`).
* Каждый следующий кубик ставится **только поверх текущего сверху**.
* Позиция по X рандомизируется в пределах **±MaxXOffsetFactor** от ширины top (по умолчанию 0.5 = 50%).
* Если достигнут потолок области (цель по `Y` не влезает в `TowerRoot.rect`) — установка запрещается.

### Дыра (слева)

* Если куб перетащен в область дыры — куб отправляется в `ObjectPool`.
* Если куб был вытащен из середины башни — кубы выше **переоцениваются правилами** и плавно падают на валидные позиции, иначе исчезают.

### Промах

* Если дроп не прошёл ни одну целевую проверку — куб улетает вниз (анимация) и отправляется в `ObjectPool`.

---

## UX: Drag vs Scroll

Чтобы не было ситуации, когда при перетаскивании происходит скролл:

* `CubeView` выбирает режим по порогу `DecideThreshold` и доминирующей оси:

  * вертикаль → `Dragging`
  * горизонталь → `Scrolling`
* На время активного перетаскивания дополнительно **блокируется** `ScrollRect.enabled` (в `BottomBarDragInteractor.LockScroll()`).

---

## Архитектура (коротко)

### Bootstrap

* `StartupPipeline<IAppStartupTask>` поднимает базовые подсистемы:

  * `ScreenRoots`
  * `LoadingOverlay` / `ISceneTransitionCurtain`,
  * загрузка конфига / локализации / прогресса,
  * переход на сцену `Core`.

### Core scene

* `StartupPipeline<ISceneStartupTask>`:

  * открывает `GameplayWindow`,
  * инициализирует `BottomBar`,
  * восстанавливает башню из прогресса.
* `SceneReadyGate` + `ISceneReadyListener`:

  * закрывает `LoadingOverlay`,
  * подгружает оверлей уведомлений (`ActionInfoOverlay`),
  * включает автосохранения.

---

## Конфиг

`GameConfigAsset : IGameConfigDefinition`

### BottomBar

* `Count` — количество элементов в нижнем скролле.
* `Colors[]` — список цветов (`id` + `sprite`).

### CubeDrag

Параметры анимаций и правил:

* `MaxXOffsetFactor`
* `TowerFallDuration`, `TowerFallEase`
* `HoleEllipsePadding`
* `HolePullDuration`, `HolePullEase`
* `HoleFallDuration`, `HoleFallEase`, `HoleFallExtra`
* `FailFallDuration`, `FailFallEase`, `FailFallExtra`

### Источники конфигурации (расширение)

Загрузка конфига завязана на `IGameConfigProvider`, поэтому источник можно заменить без изменения игровой логики.

Сейчас используется реализация `AddressablesGameConfigProvider` (`ScriptableObject` через `Addressables`), но при необходимости можно добавить альтернативные реализации, например:

* JSON/remote (TextAsset/HTTP)
* локальный файл
* любой кастомный провайдер

---

## Правила установки (точки расширения)

Правила реализованы через интерфейс:

* `ITowerPlacementRule` + `ITowerPlacementRulesEvaluator` (цепочка по `Order`).

Сейчас включены:

* `PointerInsideTowerDropAreaRule` — куб должен быть в нужной области (в т.ч. правая половина экрана + не перекрывать нижнюю панель).
* `PointerAboveTopRule` — при ручной установке низ куба должен быть выше верха верхнего куба.
* `MaxHorizontalOffsetRule` — ограничение по `X` относительно верхнего куба.

Опционально (как пример расширения):

* `SameColorAsTopRule` — ставить можно только куб того же цвета, что и верхний.

  * Включается простой регистрацией в `CoreSceneInstaller` (сейчас бинд закомментирован).

---

## Уведомления (Action info)

Действия показываются как оверлей (`ActionInfoOverlay`):

* `placed_into_tower`
* `dropped_into_hole`
* `cube_disappeared` + ошибки правил (например `height_limit_reached`)

Оверлей использует `ILocalizer` и работает по ключам локализации.

---

## Локализация

Локализация загружается из Addressables JSON (`TextAsset`) через `AddressablesJsonLocalizationProvider`.

Формат JSON (DTO):

```json
{
  "Locale": "en",
  "Entries": [
    { "Key": "bottom_bar.action.placed_into_tower", "Value": "Placed into tower" }
  ]
}
```

Поведение:

* если ключ не найден — отображается `#<key>`.

---

## Сохранение прогресса

### Где хранится

`Application.persistentDataPath/progress.json`

### Что сохраняется

* `GameProgressSnapshot` → `TowerProgressSnapshot` → список `CubePlacementSnapshot`:

  * `ColorId`
  * `LocalPosition`
  * `LocalRotation`

### Когда сохраняется

* `AutoSaveListener` подписывается на `IProgressChanged.Changed`
* сохранение с **задержкой ~300мс** после изменения (перезапуск таймера при новых изменениях)

### Восстановление

* `RestoreTowerProgressTask` создаёт кубы по `ColorId` и выставляет `anchoredPosition / localRotation`
* параллельно заполняет `TowerStackState`

---

## Пул объектов (оптимизация)

`CubeViewFactory` использует `UnityEngine.Pool.ObjectPool<CubeView>`:

* `DefaultCapacity = 32`
* `MaxPoolSize = 256`
* на `Release`:

  * уничтожает твины (`DOKill` / `DOTween.Kill`)
  * отключает `CubeView`, снимает `raycastTarget`
  * возвращает в родителя `CubePool`

---

## Используемые пакеты

* Zenject (DI)
* UniTask (async)
* DOTween (animations)
* Addressables (assets)
* R3 (reactive)