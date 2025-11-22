# Warcraft Armory Design System

## Overview
This design system provides a cohesive, accessible, and WoW-inspired visual language for the Warcraft Armory application. It supports both light and dark modes with proper contrast ratios and draws inspiration from World of Warcraft's UI aesthetic.

## Design Philosophy
- **WoW-Inspired**: Rich, deep colors reminiscent of WoW's fantasy aesthetic
- **Accessibility First**: WCAG 2.1 AA compliant contrast ratios (4.5:1 for text)
- **Clear Hierarchy**: Distinct surface elevations and color separations
- **Professional**: Clean, modern interpretation of game UI elements

---

## Color Palette

### Primary Colors (Alliance Blue)
- `--primary-50`: #E3F2FD (Light mode backgrounds)
- `--primary-100`: #BBDEFB
- `--primary-200`: #90CAF9
- `--primary-300`: #64B5F6
- `--primary-400`: #42A5F5
- `--primary-500`: #2196F3 (Main primary)
- `--primary-600`: #1E88E5
- `--primary-700`: #1976D2 (Dark mode primary)
- `--primary-800`: #1565C0
- `--primary-900`: #0D47A1

### Secondary Colors (Horde Red)
- `--secondary-50`: #FFEBEE
- `--secondary-100`: #FFCDD2
- `--secondary-200`: #EF9A9A
- `--secondary-300`: #E57373
- `--secondary-400`: #EF5350
- `--secondary-500`: #F44336 (Main secondary)
- `--secondary-600`: #E53935
- `--secondary-700`: #D32F2F (Dark mode secondary)
- `--secondary-800`: #C62828
- `--secondary-900`: #B71C1C

### Accent Colors (Epic Purple)
- `--accent-400`: #AB47BC
- `--accent-500`: #9C27B0 (Main accent)
- `--accent-600`: #8E24AA
- `--accent-700`: #7B1FA2

### Neutral Colors

#### Light Mode
- `--neutral-bg`: #FAFAFA (Page background)
- `--neutral-surface-1`: #FFFFFF (Cards, elevated surfaces)
- `--neutral-surface-2`: #F5F5F5 (Subtle backgrounds)
- `--neutral-surface-3`: #EEEEEE (Stats, badges)
- `--neutral-border`: #E0E0E0 (Dividers, borders)
- `--neutral-text-primary`: #212121 (Headings, primary text)
- `--neutral-text-secondary`: #616161 (Body text)
- `--neutral-text-tertiary`: #9E9E9E (Captions, hints)

#### Dark Mode
- `--neutral-bg-dark`: #121212 (Page background)
- `--neutral-surface-1-dark`: #1E1E1E (Cards, elevated surfaces)
- `--neutral-surface-2-dark`: #2A2A2A (Subtle backgrounds)
- `--neutral-surface-3-dark`: #363636 (Stats, badges)
- `--neutral-border-dark`: #424242 (Dividers, borders)
- `--neutral-text-primary-dark`: #FFFFFF (Headings, primary text)
- `--neutral-text-secondary-dark`: #B0B0B0 (Body text)
- `--neutral-text-tertiary-dark`: #808080 (Captions, hints)

### Semantic Colors

#### Success (Green)
- `--success-light`: #81C784
- `--success-main`: #4CAF50
- `--success-dark`: #388E3C

#### Warning (Gold)
- `--warning-light`: #FFD54F
- `--warning-main`: #FFC107
- `--warning-dark`: #FFA000

#### Error (Red)
- `--error-light`: #E57373
- `--error-main`: #F44336
- `--error-dark`: #D32F2F

#### Info (Cyan)
- `--info-light`: #4FC3F7
- `--info-main`: #29B6F6
- `--info-dark`: #0288D1

---

## WoW Class Colors
These colors are used for character-specific elements:

```scss
$class-colors: (
  'Warrior': #C79C6E,
  'Paladin': #F58CBA,
  'Hunter': #ABD473,
  'Rogue': #FFF569,
  'Priest': #FFFFFF,
  'Shaman': #0070DE,
  'Mage': #69CCF0,
  'Warlock': #9482C9,
  'Monk': #00FF96,
  'Druid': #FF7D0A,
  'Demon Hunter': #A330C9,
  'Death Knight': #C41F3B,
  'Evoker': #33937F
);
```

---

## Typography

### Font Families
- **Primary**: 'Roboto', sans-serif (body text)
- **Display**: 'Roboto Condensed', sans-serif (headings, navigation)
- **Monospace**: 'Roboto Mono', monospace (stats, numbers)

### Type Scale
- `--text-xs`: 0.75rem / 12px (captions)
- `--text-sm`: 0.875rem / 14px (small text)
- `--text-base`: 1rem / 16px (body)
- `--text-lg`: 1.125rem / 18px (emphasized)
- `--text-xl`: 1.25rem / 20px (subheadings)
- `--text-2xl`: 1.5rem / 24px (headings)
- `--text-3xl`: 2rem / 32px (page titles)
- `--text-4xl`: 2.5rem / 40px (hero text)

### Font Weights
- `--font-regular`: 400
- `--font-medium`: 500
- `--font-semibold`: 600
- `--font-bold`: 700

### Line Heights
- `--leading-tight`: 1.25
- `--leading-normal`: 1.5
- `--leading-relaxed`: 1.75

---

## Spacing System
Based on 8px grid:

```scss
$spacing: (
  '0': 0,
  '1': 0.25rem,  // 4px
  '2': 0.5rem,   // 8px
  '3': 0.75rem,  // 12px
  '4': 1rem,     // 16px
  '5': 1.25rem,  // 20px
  '6': 1.5rem,   // 24px
  '8': 2rem,     // 32px
  '10': 2.5rem,  // 40px
  '12': 3rem,    // 48px
  '16': 4rem,    // 64px
);
```

---

## Elevation & Shadows

### Light Mode Shadows
- `--shadow-sm`: 0 1px 2px rgba(0,0,0,0.05)
- `--shadow-md`: 0 4px 6px rgba(0,0,0,0.07)
- `--shadow-lg`: 0 10px 15px rgba(0,0,0,0.1)
- `--shadow-xl`: 0 20px 25px rgba(0,0,0,0.15)

### Dark Mode Shadows
- `--shadow-sm-dark`: 0 1px 2px rgba(0,0,0,0.3)
- `--shadow-md-dark`: 0 4px 6px rgba(0,0,0,0.4)
- `--shadow-lg-dark`: 0 10px 15px rgba(0,0,0,0.5)
- `--shadow-xl-dark`: 0 20px 25px rgba(0,0,0,0.6)

---

## Border Radius
- `--radius-sm`: 4px (buttons, inputs)
- `--radius-md`: 8px (cards, panels)
- `--radius-lg`: 12px (modals, large containers)
- `--radius-full`: 9999px (pills, avatars)

---

## Component Patterns

### Cards
**Light Mode:**
- Background: `--neutral-surface-1` (#FFFFFF)
- Border: 1px solid `--neutral-border` (#E0E0E0)
- Shadow: `--shadow-md`

**Dark Mode:**
- Background: `--neutral-surface-1-dark` (#1E1E1E)
- Border: 1px solid `--neutral-border-dark` (#424242)
- Shadow: `--shadow-md-dark`

### Buttons

#### Primary Button
**Light Mode:**
- Background: `--primary-600` (#1E88E5)
- Text: #FFFFFF
- Hover: `--primary-700` (#1976D2)
- Border Radius: `--radius-sm`

**Dark Mode:**
- Background: `--primary-700` (#1976D2)
- Text: #FFFFFF
- Hover: `--primary-600` (#1E88E5)

#### Secondary Button
**Light Mode:**
- Background: transparent
- Border: 1px solid `--neutral-border`
- Text: `--neutral-text-primary`
- Hover: Background `--neutral-surface-2`

**Dark Mode:**
- Background: transparent
- Border: 1px solid `--neutral-border-dark`
- Text: `--neutral-text-primary-dark`
- Hover: Background `--neutral-surface-2-dark`

### Inputs
**Light Mode:**
- Background: #FFFFFF
- Border: 1px solid `--neutral-border`
- Focus Border: `--primary-600`
- Text: `--neutral-text-primary`
- Placeholder: `--neutral-text-tertiary`

**Dark Mode:**
- Background: `--neutral-surface-2-dark`
- Border: 1px solid `--neutral-border-dark`
- Focus Border: `--primary-700`
- Text: `--neutral-text-primary-dark`
- Placeholder: `--neutral-text-tertiary-dark`

### Navigation
**Light Mode:**
- Background: `--primary-700` (#1976D2)
- Text: #FFFFFF
- Active Item: rgba(255,255,255,0.2) background

**Dark Mode:**
- Background: `--neutral-surface-1-dark` (#1E1E1E)
- Border Bottom: 2px solid `--primary-700`
- Text: `--neutral-text-primary-dark`
- Active Item: `--primary-700` background

### Stat Boxes
**Light Mode:**
- Background: `--neutral-surface-3` (#EEEEEE)
- Border: none
- Text: `--neutral-text-primary`
- Icon: `--primary-600`

**Dark Mode:**
- Background: `--neutral-surface-3-dark` (#363636)
- Border: 1px solid `--neutral-border-dark`
- Text: `--neutral-text-primary-dark`
- Icon: `--primary-700`

---

## Accessibility Guidelines

### Contrast Ratios
- Normal text: minimum 4.5:1
- Large text (18px+): minimum 3:1
- UI components: minimum 3:1

### Focus States
- Outline: 2px solid `--primary-600` (light) or `--primary-700` (dark)
- Offset: 2px
- Always visible for keyboard navigation

### Color Usage
- Never rely on color alone to convey information
- Use icons, labels, or patterns in addition to color
- Test with colorblindness simulators

---

## Implementation Notes

### Dark Mode Toggle
- Store preference in localStorage
- Apply `data-theme="dark"` attribute to `<html>` element
- Use CSS custom properties for theme-aware styling
- Provide smooth transitions between themes

### Responsive Breakpoints
- `xs`: 0px - 599px (mobile)
- `sm`: 600px - 959px (tablet)
- `md`: 960px - 1279px (small desktop)
- `lg`: 1280px - 1919px (desktop)
- `xl`: 1920px+ (large desktop)

### Performance
- Use CSS custom properties for theme values
- Minimize style recalculations during theme switches
- Lazy load theme-specific assets when possible

---

## Migration Checklist

- [ ] Create `_variables.scss` with all color tokens
- [ ] Create `_mixins.scss` for reusable patterns
- [ ] Update `styles.scss` with theme implementation
- [ ] Update all component `.scss` files to use design tokens
- [ ] Implement dark mode toggle in header
- [ ] Test color contrast ratios
- [ ] Verify responsive behavior
- [ ] Test with screen readers
- [ ] Document component usage examples
