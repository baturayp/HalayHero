# HalayHero
A Unity rhthym game based on [Boots-cuts](https://github.com/YuChaoGithub/boots-cuts)

Required packages:
 - TextMeshPro 2.1
 - Cinemachine

## 3rd party libraries and licenses
- [Icons](https://icons8.com/icon/pack/free-icons/plasticine)
- [HVD Comic Serif Font](https://www.dafont.com/hvd-comic-serif.font)

## Note Editor
![](NoteEditor/help.png?raw=true)

### Usage instructions
Make sure you put your .wav files inside Assets/Resources/Sounds/Musics folder. Note Editor creates a .json file in Assets/Resources/Sounds/Notes folder when you edit/save a song.

- Control+Drag => Range selection, select multiple beats on canvas
- Shift+LeftClick => Start drawing a long beat
- Right-click => Delete
- Right-Left arrow keys => move position
- Space => Play/pause
- Ctrl-c & Ctrl-v => Copy & paste
- Ctrl-z & Ctrl-r => Undo & redo
- Increase resolution if you want more precision, but note that existing beats can be edited on the same resolution-level. However you can select any beat with range-selection control (Ctrl+Drag) and delete them with 'Del' keyboard button.
- You can edit a long-beat's size by clicking its head or delete it by clicking its tail (when long-beat editing is active).
- Increase a multi-beat with left-click, decrease with shift-click, delete with right-click.
- Mp3 editing not supported beause of unity's license restrictions. Only .wav files allowed. Anyway either wav or mp3 files are compressed as ogg when imported into the game. So better save your clips as wav files if you want to use them in the note editor.
