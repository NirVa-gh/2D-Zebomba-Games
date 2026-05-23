
# Проект "Take an apple"

<div align="center">
  <img width="690" height="390" alt="MatchReview" src="https://github.com/user-attachments/assets/500e49ed-3564-4c2d-9ff6-95e7937a4c9c" />
</div>

## Описание проекта

### Система игрока:
Контроллер персонажа с физикой 

Мобильное управление через виртуальный джойстик

Система здоровья и получения урона
Система опыта (XP) и прокачки
###Боевая система:

Оружие ближнего и дальнего боя
Авто-прицеливание по врагам
Критические удары
Система хитбоксов через BoxCollider2D
Визуализация урона (damage text particles)
### Система врагов:

Melee и Ranged враги
Волны спавна (Wave Manager)
Индикатор спавна
Object Pooling для оптимизации
### Оптимизация:

Object Pooling для частиц урона
Правильная работа с физикой и коллизиями
Использованные паттерны и технологии:
State Machine - управление состояниями ИИ
Observer Pattern - события через Actions/Events
Object Pooling - переиспользование объектов
ScriptableObject - data-driven конфигурация
Component-based architecture - модульная архитектура Unity
Dependency Injection - через [SerializeField] и GetComponent


