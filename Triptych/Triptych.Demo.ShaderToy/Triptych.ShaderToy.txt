//See https://www.shadertoy.com/view/Xstyz2

const bool SHOW_LINES = true;
const float LINE_THICKNESS = .003;
const vec4 LINE_COLOR = vec4(0); //black

const bool FLIP_HORIZ = false;

const float FADE = .0; //try .1 or .2


void mainImage(out vec4 fragColor, in vec2 fragCoord)
{
  vec2 p = fragCoord.xy/iResolution.xy;
  float x2;
  vec4 col;

  //Left
  if (p.x < .25)
  {
    x2 = p.x+.25; //horiz flip: .25-p.x;
    if (FLIP_HORIZ) x2 = .5-x2;      
    col = texture(iChannel0, vec2(x2, p.y)) + FADE; //do color effect
  }

  //Middle
  else if (p.x < .75)
  {
    x2 = .75-p.x; //horiz flip: p.x-.25
    if (FLIP_HORIZ) x2 = .5-x2;      
    col = texture(iChannel0, vec2(x2, p.y)); //could do some color effect here too
  }

  //Right
  else
  {
    x2 = p.x-.75; //horiz flip: 1.25-p.x
    if (FLIP_HORIZ) x2 = .5-x2;
    col = texture(iChannel0, vec2(x2, p.y)) + FADE; //do color effect
  }

  //Lines (optional)
  if (SHOW_LINES && (abs(.25-p.x)<LINE_THICKNESS || abs(.75-p.x)<LINE_THICKNESS))
    col = LINE_COLOR;

  fragColor = col;
}
