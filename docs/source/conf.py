# Configuration file for the Sphinx documentation builder.
#
# For the full list of built-in configuration values, see the documentation:
# https://www.sphinx-doc.org/en/master/usage/configuration.html

# -- Project information -----------------------------------------------------
# https://www.sphinx-doc.org/en/master/usage/configuration.html#project-information

project = 'TeleFlow'
copyright = '2026, Gleb Bannyy'
author = 'Gleb Bannyy'
release = '0.1'

# -- General configuration ---------------------------------------------------
# https://www.sphinx-doc.org/en/master/usage/configuration.html#general-configuration

extensions = [
    "sphinx.ext.githubpages",
    'sphinx.ext.graphviz'
]

templates_path = ['_templates']
exclude_patterns = []

language = 'ru'

# -- Options for HTML output -------------------------------------------------
# https://www.sphinx-doc.org/en/master/usage/configuration.html#options-for-html-output

html_theme = 'sphinx_rtd_theme'

html_theme_options = {
    "collapse_navigation": False,   # не схлопывать дерево до текущей страницы
    "navigation_depth": 6,          # насколько глубоко показывать уровни в toctree
    "sticky_navigation": True,      # чтобы левое меню не прыгало
    "includehidden": True, 
    "titles_only": True,         # ВАЖНО: включить :hidden: toctree в навигацию
}

html_static_path = ['_static']
