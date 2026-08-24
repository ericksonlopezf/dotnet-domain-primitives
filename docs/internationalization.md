# Internationalization & Culture Invariance (i18n)

---

## 1. Culture Invariance

All source-generated string and numeric parsers (`IParsable<TPrimitive>`, `ISpanParsable<TPrimitive>`) enforce `CultureInfo.InvariantCulture` by default when no format provider is passed.
