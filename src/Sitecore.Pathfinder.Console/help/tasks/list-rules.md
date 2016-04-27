list-rules
===============
Lists the available conditions and actions.

Remarks
-------
The `list-rules` tasks outputs a list of all the available conditions and actions for rules.

Settings
--------
None.

Example
-------
```cmd
> scc list-rules

CONDITIONS:
absolute-file-name
always
children
database-name
directory-exists
field-compiled-value
field-source
field-type
field-value
file-exists
file-name
file-name-directory
file-name-extension
file-name-extensions
has-parent-with-name
has-template-field
has-template-section
is-field-empty
is-field-shared
is-field-unversioned
item-icon
item-id-or-path
item-name
item-version-count
parent-item-name
parent-item-path
parent-item-template-name
project-file-name
qualified-name
reference-count
short-name
template-base-templates
template-field-count
template-icon
template-id-or-path
template-long-help
template-name
template-section-count
template-section-field-count
template-short-help
usage-count

ACTIONS:
abort
trace-error
trace-information
trace-warning```

