LFD Image Reader
===============================================================================
Author: Michael Gaisser (mjgaisser@gmail.com)
Version 1.2
Date: 2020.06.13
===============================================================================

This editor reads and exports the DELT image and ANIM animation resource types
found in LFD resource files.

===================
VERSION HISTORY

v1.2 - 13 Jun 2020
 - Release

===================
INSTRUCTIONS

- LIR will attempt to detect a TIE95 (remastered) installation; either an
original install configuration, the Markus Egger MSI installation, or via
Steam. If one cannot be detected, it will prompt you for a TIE executable to
locate the installation. This is required to load default color palettes. This
prompt will allow you to select the original and CD releases.
- Open the LFD you wish to search using the 'Open' button. The full path will be
displayed next to it. 
- Select the DELT or ANIM resource you wish to view and hit the 'Show' button,
or double-click to load immediately.
- If an animation, use the left and right buttons to switch animation frames.
- Use the 'Save' button to export the image.

===================
IMAGE NOTES

Due to how TIE layers color palettes, some images will appear... interesting.
This is due to the colors being defined separately and sometimes it takes
multiple palettes to make up a given image. This utility first loads the
default 16 colors, then if the LFD is a SHIP or BATTLE variant layers the
specific palette for those, otherwise loads every palette found in the selected
LFD.

Some images will look fine, others will obviously have the wrong colors; either
they were never defined, or were overwritten by a later palette. Without
getting into layouts (which is the purpose of TIE Layout Editor) there isn't a
good way to determine what is appropriate for a given image.

For animations, they will be shown as the full extents of the animation,
however when exported the frame will be only the defined size. A good example
of this is CITY.LFD. Both ANIMtie07 and ANIMshuttle2 are rather large overall,
however individual frames are fairly small and move across the screen.

===============================================================================
Copyright © 2008-2020 Michael Gaisser
This program and related files are licensed under the Mozilla Public License.
See License.txt for the full text. If for some reason the license was not
distributed with this program, you can obtain the full text of the license at
http://mozilla.org/MPL/2.0/.

"Star Wars" and related items are trademarks of LucasFilm Ltd and
LucasArts Entertainment Co.

This software is provided "as is" without warranty of any kind; including that
the software is free of defects, merchantable, fit for a particular purpose or
non-infringing. See the full license text for more details.