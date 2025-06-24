# TodoApi - REST API для управления задачами (TODO)

![.NET Core](https://img.shields.io/badge/.NET-8.0-blue)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-purple)
![Entity Framework](https://img.shields.io/badge/Entity_Framework-8.0-green)

## 📌 О проекте

Это RESTful API для управления списком задач (TODO), разработанное на ASP.NET Core с использованием Entity Framework Core. API предоставляет стандартные CRUD операции для работы с задачами.

## 🛠 Технологии

- **ASP.NET Core 8** - Веб-фреймворк для создания API
- **Entity Framework Core 8** - ORM для работы с базой данных
- **SQLite** - Встроенная база данных (также доступна InMemory для тестирования)
- **AutoMapper** - Для маппинга между моделями и DTO
- **Swagger** - Для документации и тестирования API

## 📋 Функционал

API поддерживает следующие операции с задачами:

- Получение списка всех задач
- Получение задачи по ID
- Создание новой задачи
- Обновление существующей задачи
- Удаление задачи

## 🚀 Запуск проекта

1. Клонируйте репозиторий:
   ```bash
   git clone https://github.com/Dalkory/TodoApi.git
   ```

2. Запустите проект:
  - Откройте файл **TodoApi.sln**

## 📊 Модель данных

```csharp
public class TodoItem
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Title { get; set; }
    
    public bool IsCompleted { get; set; }
    
    public DateTime CreatedAt { get; set; }
}
```

## 🌐 API Endpoints

| Метод | URL                | Описание                     |
|-------|--------------------|------------------------------|
| GET   | /api/todos         | Получить список всех задач   |
| GET   | /api/todos/{id}    | Получить задачу по ID        |
| POST  | /api/todos         | Создать новую задачу         |
| PUT   | /api/todos/{id}    | Обновить существующую задачу |
| DELETE| /api/todos/{id}    | Удалить задачу               |

## 🎯 Особенности реализации

- **Валидация данных**: Использованы атрибуты `[Required]` и `[StringLength]`
- **DTO и AutoMapper**: Для передачи данных между слоями приложения
- **Логгирование**: Встроенное логгирование действий
- **Обработка ошибок**: Глобальная обработка ошибок через middleware
- **Тестирование**: Включены unit-тесты для контроллера

## 🧪 Тестирование

Проект включает unit-тесты для проверки функциональности API.

## 📝 Лицензия

Этот проект распространяется под лицензией MIT. Подробнее см. в файле [LICENSE](LICENSE).

---

Разработано с ❤️ для тестового задания на ASP.NET Core
