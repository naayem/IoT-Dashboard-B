# User Stories - IoT Dashboard Project

## User Story 1: Affichage des données en temps réel

- **En tant que** gestionnaire IoT,
- **je veux** visualiser les données des capteurs (température et humidité) en temps réel sur un tableau de bord,
- **pour** pouvoir surveiller l’état des appareils.

### Critères d'acceptation:
1. Le tableau de bord doit afficher un tableau avec l’ID de l’appareil, la température, l’humidité et l’horodatage.
2. Les données doivent être mises à jour toutes les 5 secondes.
3. Le tableau doit être vide si aucun appareil n’est connecté.

---

## User Story 2: Alerte en cas de déconnexion

- **En tant que** gestionnaire IoT,
- **je veux** recevoir une alerte lorsqu'un appareil est hors ligne,
- **pour** pouvoir intervenir rapidement en cas de problème.

### Critères d'acceptation:
1. Une alerte doit être déclenchée si un appareil n'envoie pas de données pendant plus de 10 secondes.
2. L’alerte doit inclure l’ID de l’appareil déconnecté.
