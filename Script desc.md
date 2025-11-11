# Script desc

WHY?
You want the text to start unreadable (by shifting the material UV offset so glyphs look misaligned/jittery) and then — after a short period — return to normal so the text becomes readable.

Animate a material shader property that controls the texture UV offset.

While the offset is non-zero (and jittery), glyphs sample the font atlas from shifted UVs → unreadable.

Over time, move the offset back to (0,0) so sampling returns to normal and text becomes readable.

This uses the material’s texture offset (a vector that shifts how the texture is sampled). For UI/TextMeshPro this works because glyphs are sampled from an SDF texture atlas.