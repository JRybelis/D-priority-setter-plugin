# D-priority-setter-plugin
Plugin to Microsoft Dynamics

Plugin acts on Employee Account Entity:
**WHEN** Employee Account is Created or Updated
**AND** priority field is changing its value
**THEN** the plugin calculates the priority of Account record, linked to the employee, by choosing the lowest priority value out of all Employee Account records that are linked to the Account.
**AND** updates the priority field of the linked Account record.

Employee Account has a lookup field to an Account entity.
The “Priority“ field in Employee Account is an optionSet with the following possible values: 
1, 2, 3, 4, 5.

Priority of value 1 is considered to be the highest.

Priority of value 5 is considered to be the lowest.

*E.g.: If Account has an Employee Account with Priority 3 and an Employee Account with Priority 1, then priority of the incoming Employee Account record should be set to 1.*
