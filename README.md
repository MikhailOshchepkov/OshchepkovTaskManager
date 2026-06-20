# OshchepkovTaskManager

Менеджер задач — учебный проект (летняя практика, 3 курс).

## Структура решения

```
OshchepkovTaskManager.sln
├── OshchepkovTaskManager.Core       — библиотека бизнес-логики (DLL)
│   ├── Models/
│   │   ├── TaskItem.cs              — модель задачи
│   │   └── TaskStatistics.cs        — модель статистики
│   ├── Interfaces/
│   │   ├── ITaskRepository.cs       — интерфейс репозитория
│   │   └── ITaskService.cs          — интерфейс сервиса
│   └── Services/
│       ├── TaskRepository.cs        — хранение в памяти + JSON I/O
│       └── TaskService.cs           — бизнес-логика, LINQ, статистика
│
├── OshchepkovTaskManager.App        — WPF-приложение (MVVM)
│   ├── Views/
│   │   ├── MainWindow.xaml/.cs      — главное окно
│   │   └── TaskEditWindow.xaml/.cs  — диалог добавления/редактирования
│   ├── ViewModels/
│   │   ├── ViewModelBase.cs         — INotifyPropertyChanged
│   │   ├── RelayCommand.cs          — ICommand
│   │   ├── MainViewModel.cs         — ViewModel главного окна
│   │   └── TaskEditViewModel.cs     — ViewModel диалога
│   └── Converters/
│       └── Converters.cs            — конвертеры значений для XAML
│
└── OshchepkovTaskManager.Tests      — модульные тесты (xUnit)
    ├── TaskItemTests.cs
    ├── TaskRepositoryTests.cs
    └── TaskServiceTests.cs
```

## Требования

- .NET 8 SDK
- Visual Studio 2022 (с компонентом «Разработка классических приложений .NET»)
- Windows (WPF работает только на Windows)

## Быстрый старт

1. Открыть `OshchepkovTaskManager.sln` в Visual Studio 2022
2. Выбрать стартовый проект: `OshchepkovTaskManager.App`
3. Нажать **F5** для запуска

## Запуск тестов

```
dotnet test OshchepkovTaskManager.Tests
```
или через Visual Studio: **Test → Run All Tests**

## Реализованные требования

| №    | Требование                                          | Статус |
|------|-----------------------------------------------------|--------|
| 1.1  | Добавление задачи (название, описание, ...)         | ✅     |
| 1.2  | Просмотр списка задач                               | ✅     |
| 1.3  | Фильтрация по статусу                               | ✅     |
| 1.4  | Поиск по названию / описанию                        | ✅     |
| 1.5  | Редактирование задачи                               | ✅     |
| 1.6  | Удаление задачи                                     | ✅     |
| 1.7  | Сохранение / загрузка JSON                          | ✅     |
| 1.8  | Логика в DLL, UI — WPF                              | ✅     |
| 1.9  | Модульные тесты (xUnit)                             | ✅     |
| 2.1  | Сортировка по приоритету и сроку                    | ✅     |
| 2.2  | Пометка «Важная» (★, цвет)                          | ✅     |
| 2.3  | Статистика (всего / новых / в процессе / просрочено)| ✅     |
