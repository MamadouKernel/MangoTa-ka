# Charte Graphique - Mango Taika District

## 🎨 Palette de Couleurs du Logo

La palette de couleurs est basée sur les couleurs du logo : **VERT & BLANC**

### Couleurs Principales

| Couleur | Code Hex | Usage Principal |
|---------|----------|-----------------|
| **#2D5016** | ![#2D5016](https://via.placeholder.com/50x50/2D5016/FFFFFF?text=+) | Vert foncé principal - Navbar, boutons primaires, textes |
| **#4A7C2A** | ![#4A7C2A](https://via.placeholder.com/50x50/4A7C2A/FFFFFF?text=+) | Vert moyen - Hovers, accents |
| **#6BA644** | ![#6BA644](https://via.placeholder.com/50x50/6BA644/FFFFFF?text=+) | Vert clair - Variantes, éléments secondaires |
| **#8FCB6B** | ![#8FCB6B](https://via.placeholder.com/50x50/8FCB6B/000000?text=+) | Vert très clair - Backgrounds, accents légers |
| **#B8E09A** | ![#B8E09A](https://via.placeholder.com/50x50/B8E09A/000000?text=+) | Vert le plus clair - Highlights, effets |
| **#E8F5E1** | ![#E8F5E1](https://via.placeholder.com/50x50/E8F5E1/000000?text=+) | Vert très doux - Backgrounds, zones claires |
| **#FFFFFF** | ![#FFFFFF](https://via.placeholder.com/50x50/FFFFFF/000000?text=+) | Blanc - Fond, textes sur fond sombre |
| **#FAFAFA** | ![#FAFAFA](https://via.placeholder.com/50x50/FAFAFA/000000?text=+) | Blanc doux - Backgrounds alternatifs |
| **#1A3009** | ![#1A3009](https://via.placeholder.com/50x50/1A3009/FFFFFF?text=+) | Vert très foncé - Textes principaux |
| **#6B7A5F** | ![#6B7A5F](https://via.placeholder.com/50x50/6B7A5F/FFFFFF?text=+) | Gris-vert - Textes secondaires |
| **#D4E4C8** | ![#D4E4C8](https://via.placeholder.com/50x50/D4E4C8/000000?text=+) | Vert très clair - Bordures, séparateurs |

### ✅ Application Complète

**Toutes les couleurs utilisées dans l'application correspondent EXACTEMENT aux couleurs du logo (VERT & BLANC).**

## 📍 Utilisation des Couleurs dans le Code

### Variables CSS (définies dans `:root`)
Toutes les couleurs sont centralisées via des variables CSS pour faciliter la maintenance :

```css
:root {
  --color-primary: #2D5016;           /* Vert foncé principal */
  --color-primary-medium: #4A7C2A;    /* Vert moyen */
  --color-primary-light: #6BA644;     /* Vert clair */
  --color-primary-lighter: #8FCB6B;   /* Vert très clair */
  --color-primary-lightest: #B8E09A;  /* Vert le plus clair */
  --color-primary-soft: #E8F5E1;      /* Vert très doux */
  --color-white: #FFFFFF;             /* Blanc */
  --color-white-soft: #FAFAFA;        /* Blanc doux */
  --color-text-dark: #1A3009;         /* Vert très foncé */
  --color-text-medium: #2D5016;       /* Vert foncé */
  --color-text-light: #6B7A5F;        /* Gris-vert */
  --color-border: #D4E4C8;            /* Vert très clair */
}
```

### Correspondances rgba (pour transparences)
Lorsque des effets de transparence sont nécessaires, les couleurs rgba correspondent exactement :

- `rgba(45, 80, 22, ...)` = #2D5016 (Vert foncé)
- `rgba(74, 124, 42, ...)` = #4A7C2A (Vert moyen)
- `rgba(107, 166, 68, ...)` = #6BA644 (Vert clair)
- `rgba(143, 203, 107, ...)` = #8FCB6B (Vert très clair)
- `rgba(184, 224, 154, ...)` = #B8E09A (Vert le plus clair)
- `rgba(232, 245, 225, ...)` = #E8F5E1 (Vert très doux)
- `rgba(255, 255, 255, ...)` = #FFFFFF (Blanc)

## 🎨 Polices

- **Poppins** (Google Fonts) - Chargée via CDN
- **Myriad Pro** - Fallback système
- Font-weight: 600 pour les titres (h1-h6)

## ✅ Vérification Complète

Toutes les couleurs utilisées dans l'application correspondent EXACTEMENT aux couleurs du logo :

### Fichiers Vérifiés
- ✅ `Views/Public/Home/Index.cshtml` - Page d'accueil
- ✅ `Views/Shared/_Layout.cshtml` - Layout principal
- ✅ `Views/Admin/Dashboard/Index.cshtml` - Dashboard admin
- ✅ `wwwroot/css/site.css` - Styles globaux
- ✅ `wwwroot/css/modern-components.css` - Composants modernes
- ✅ Tous les autres fichiers de vues

### Règles Appliquées
1. **Aucune couleur en dehors de la palette VERT & BLANC** n'est utilisée
2. **Tous les gradients** utilisent uniquement les nuances de vert et blanc
3. **Tous les graphiques** (Chart.js) utilisent la palette verte
4. **Toutes les bordures et ombres** respectent la palette
5. **Variables CSS centralisées** pour faciliter la maintenance

## 🎯 Résultat

**100% de conformité avec les couleurs du logo (VERT & BLANC)**
