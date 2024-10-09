# Project Definition Protocol

## Steps to follow

### Step 1: Colelct and organize stakeholder's ideas
When Ideas are vague and confused, the first thing to do is to organize a clarification session.
1. Listen Actively:
    - Listen actively what the stakeholders have to say without interrompting. Even if some things are ambiguous or non-technical
    - Take notes of all ideas. Whithout trying to rectify them or to structure them immidiately.
2. Ask Open Questions:
    - "Can you explain what interest you the most in this functionnality?"
    - "What results do you expect exactly?"
3. Identify the business objectives:
    - Always ask why this idea is important for their business or project. This help devising the core objectives:
        - Is it to improve user experience?
        - Is it to automatize an internal process?
4. Reformulate key points:
    - After listening reformulate what you understood. This helps ensuring that the needs are well understood. Even if stakeholders didn't articulate them well.

### Step 2: Transform Ideas into clear requirements

1. Redact **User Stories**:
    - Use **User Stories** technique to encapsulate the needs in concise formulation. Every User Story use the following model:
        - As a [type of user], I want [Functionnality] for [Objective].
2. Define Acceptation criterias.
    - Every User Stories have Acceptation criterias detailing how the functionnality will be validated. Exemple:
        - Critères d'acceptation : Le gestionnaire doit pouvoir visualiser un tableau contenant les valeurs de température et d’humidité mises à jour toutes les 5 secondes.
    - Ces critères aident aussi à définir les tests que tu écriras dans le cadre du TDD.
3. Prioritize requirements:
    - Classify User Stories by priority in function of the core stakeholders objectives. Use of tool such as MoSCoW (Must have, Should have, Could have, Won't have).
4. Clarifier les contraintes techniques:
    - Identifie technologies, platforms or standard specificities required by the project. For exemple, if the application need to be integrated with cloud specific ou use specific type of DB.

### Step 3: TDD
Étape 3 : Rédiger les tests avant de coder (TDD)
Maintenant que tu as des exigences claires, tu peux les transformer en tests pour appliquer une approche TDD.

Écrire les tests :

Chaque User Story et chaque critère d'acceptation doit correspondre à un test. Ces tests peuvent être des tests unitaires, des tests d’intégration, ou des tests fonctionnels.
Exemple de test unitaire en C# pour vérifier qu’un tableau affiche les données des appareils :

csharp
Copier le code
[Fact]
public void Should_Display_Device_Data_When_Devices_Exist()
{
    // Arrange
    var deviceService = new DeviceService();
    deviceService.AddDevice(new Device { Id = "device-001", Temperature = 25.5, Humidity = 60 });
    
    // Act
    var result = deviceService.GetDeviceData();
    
    // Assert
    Assert.NotEmpty(result);
    Assert.Equal("device-001", result.First().Id);
    Assert.Equal(25.5, result.First().Temperature);
}
Exécuter les tests :

Dans l’approche TDD, tu écris d’abord les tests (qui vont échouer car le code n’existe pas encore). Ensuite, tu écris juste assez de code pour passer les tests.
Coder, tester, et améliorer :

Une fois que le test passe, tu peux refactoriser et améliorer ton code. Le cycle Red (échec du test) - Green (réussite du test) - Refactor est au cœur du TDD.
Exemple d’application de cette méthodologie dans ton projet IoT
Idées confuses des stakeholders :
Stakeholder A : "On aimerait voir toutes les données des capteurs en temps réel, mais aussi pouvoir recevoir des alertes."
Stakeholder B : "Je veux pouvoir savoir si un appareil est déconnecté rapidement."
User Stories correspondantes :
User Story 1 :

En tant que gestionnaire IoT, je veux visualiser les données des capteurs en temps réel pour surveiller les conditions des appareils.
Critères d'acceptation : Un tableau affiche les données de température et d'humidité mises à jour toutes les 5 secondes.
User Story 2 :

En tant que gestionnaire IoT, je veux recevoir une alerte quand un appareil est hors ligne pour pouvoir intervenir rapidement.
Critères d'acceptation : Une notification est envoyée si un appareil n’envoie pas de données pendant plus de 10 secondes.*
Tests à écrire pour la première User Story (TDD) :
Écrire un test pour s’assurer que les données des capteurs sont affichées en temps réel.
Écrire un test pour vérifier que les données sont mises à jour toutes les 5 secondes.
Ensuite, tu codes juste assez pour faire passer ces tests avant d'ajouter d'autres fonctionnalités.

