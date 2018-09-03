# Monster Hunter: World - DPS Overlay

This mod displays the damage breakdown and DPS of your current team. 

During hunts, this mod displays the percent of total team damage done and the [exponential moving average](https://en.wikipedia.org/wiki/Moving_average#Exponential_moving_average) of DPS of each player. After hunts, this mod displays the damage, percent of total team damage, and the overall DPS of each player.


## Instructions

1. Run Monster Hunter: World
2. Run this mod and adjust the position and size of the overlay
3. Enjoy the hunt!

## Known Limitations/Bugs

- Post-hunt DPS is low because it includes the 60/20 seconds period after the hunt and the time spent in the post-hunt summary pages (possibly).

## Credits

[Original mod](https://www.nexusmods.com/monsterhunterworld/mods/88) by [hqvrrsc4](https://www.nexusmods.com/monsterhunterworld/users/7950104). Modified with permission.

## Todo 

### Basic Functionality
- [x] Implement DPS calculation
- [x] Fix numbers for subsequent hunts
- [ ] Fix post hunt DPS calculation. Maybe store the last damage timestamp and use that as the end time for calculating overall DPS.

### UI/UX
- [ ] Fix character clipping
- [ ] Improve overlay UI

### Misc.
- [ ] Publish to NexusMods
- [ ] Add a license to this repo
- [ ] Add more details and screenshots to the README
