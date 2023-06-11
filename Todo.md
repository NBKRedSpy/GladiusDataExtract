# Issues
	Flesh hooks are not showing a value.
# PBI
- Filter by faction (Orks, neutral, etc.).  
	Maybe Something like Units/Orks (besides just a regular filter)

# Backlog Candidates  
- Fix icons for traits that require an upgrade.
	The icons are part of the trait data, unlike regular traits.
	
# Backlog
- #B1 Filter clicks enable/disable an upgrade.  
	The user clicks an upgrade icon and that upgrade is considered researched.
	Highlight that icon for every unit and apply the effects to the unit.
		May also highlight the values that are changed from upgrades.

- #B2 Add display for permutation of traits?  Maybe have the option to select which upgrades to include in the weapon stats?
	Similar to #B1

- #B3 Apply traits to weapons.  
	#B1, #B2 are related.
    * *Warning* some traits are based on something being unlocked. See [Trait View Options](#trait-view-options)
	
        * figure out how to handle that for view.
	Example:  Hive Tyrant smash and rending
	Might do anything that does have a trait/modifiers/modifier/conditions for now.  Really only care about the weapons.

	I think there are some conditional trait applications as well.

- Add item compare.  Comparing weapons to weapons or unit to unit.
	- weapons to weapons are probably more difficult.

- Add option to show text instead of icons (mostly for searching purposes)
	- hotkey or dropdown menu

- Add descriptions to tool tips.  For example:  SistersOfBattle/SisterSuperior:
```xml
	<entry name="SistersOfBattle/SisterSuperior" value="Sister Superior"/>
	<entry name="SistersOfBattle/SisterSuperiorDescription" value="Increases the morale of infantry and Paragon Warsuits units."/>
```

- Add hot map for values? For example, show a colored value for very high and very low values. 
	Useful for large model counts which have a lot of damage output

- [ ] Add the remaining unit base values to the display (currently not displayed).  I think Cargo and moral are examples.
