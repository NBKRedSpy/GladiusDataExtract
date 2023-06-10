
# Backlog Candidates  
- Fix icons for traits that require an upgrade.
	The icons are part of the trait data, unlike regular traits.
	
# Backlog
- Filter by faction (Orks, neutral, etc.).  
	Maybe Something like Units/Orks (besides just a regular filter)

- Add descriptions to tool tips.  For example:  SistersOfBattle/SisterSuperior:
```xml
	<entry name="SistersOfBattle/SisterSuperior" value="Sister Superior"/>
	<entry name="SistersOfBattle/SisterSuperiorDescription" value="Increases the morale of infantry and Paragon Warsuits units."/>
```

- Add hot map for values? For example, show a colored value for very high and very low values. 
	Useful for large model counts which have a lot of damage output

- Add display for permutation of traits?  Maybe have the option to select which upgrades to include in the weapon stats?

- Add item compare.  Comparing weapons to weapons or unit to unit.

- [ ] Apply traits to weapons.  
    * *Warning* some traits are based on something being unlocked. See [Trait View Options](#trait-view-options)
	
        * figure out how to handle that for view.
	Example:  Hive Tyrant smash and rending
	Might do anything that does have a trait/modifiers/modifier/conditions for now.  Really only care about the weapons.

	I think there are some conditional trait applications as well.

- [ ] Add the remaining unit base values to the display (currently not displayed).  I think Cargo and moral are examples.


## Trait view options
Maybe display a weapon for each permutation for traits?  Not sure on that.

Might be easier if they are inline?

Ultimately a javascript filter would be best.


# Features
* Add item compare

