MGFX \�[�  *  #ifdef GL_ES
precision mediump float;
precision mediump int;
#endif

vec4 ps_r0;
uniform sampler2D ps_s0;
varying vec4 vTexCoord0;
#define ps_t0 vTexCoord0
#define ps_oC0 gl_FragColor

void main()
{
	ps_r0 = texture2D(ps_s0, ps_t0.xy);
	ps_r0.xyz = ps_r0.xxx;
	ps_oC0 = ps_r0;
}

    ps_s0   s0      
Technique1 Pass1 �    