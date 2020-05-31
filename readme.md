# HalayHero
A Unity rhthym game based on [Boots-cuts](https://github.com/YuChaoGithub/boots-cuts)

Required packages:
 - TextMeshPro 2.1
 - Cinemachine

## 3rd party libraries and licenses
- [Icons](https://icons8.com/icon/pack/free-icons/plasticine)
- [HVD Comic Serif Font](https://www.dafont.com/hvd-comic-serif.font)

## NoteEditor
![](NoteEditor/help.png?raw=true)

### Usage instructions
Make sure you put your musics (as .wav files) inside Assets/Resources/Sounds/Musics folder. NoteEditor creates a .json file in Assets/Resources/Sounds/Notes folder when you edit/save a song.

- Ctrl + Drag => Range selection, select multiple notes on canvas
- Shift + Left-Click => Start drawing a long note
- Right-click / Del => Delete a note
- Right-Left arrows / mouse wheel => move position
- Space => Play / pause
- Ctrl-c & Ctrl-v => Copy & paste
- Ctrl-z & Ctrl-r => Undo & redo
- You can adjust number of blocks (which default 3 for HalayHero) and assign them keyboard shortcuts (default A,S,D). It allows you to add new notes while playing audio with corresponding keys. Only works with single-notes.
- Increase resolution if you want more precision, but note that existing notes can be edited on the same resolution-level. However you can select any note with range-selection control (Ctrl + Drag) and delete them by pressing 'Del' on keyboard.
- You can edit a long-note's size by clicking its head or delete it by clicking its tail (when long-note editing is active).
- Increase a multi-note's value with left-click, decrease with (Shift + right-click), delete with right-click.
- Mp3 editing not supported beause of unity's license restrictions. Only .wav files allowed. Anyway either wav or mp3 files are compressed as ogg when imported into the game. So better save your clips as wav files if you want to use them in the NoteEditor.
- Changing BPM has no effect on gameplay (a note's strike position is the exact time value expressed as seconds.miliseconds when you hover over a note) If you want to notes appear and move faster or slower in game, adjust tempo value inside HalayHero. Though, it's still a good practice to find a song's natural BPM and set it accordingly for better precision.