/* To avoid CSS expressions while still supporting IE 7 and IE 6, use this script */
/* The script tag referencing this file must be placed before the ending body tag. */

/* Use conditional comments in order to target IE 7 and older:
	<!--[if lt IE 8]><!-->
	<script src="ie7/ie7.js"></script>
	<!--<![endif]-->
*/

(function() {
	function addIcon(el, entity) {
		var html = el.innerHTML;
		el.innerHTML = '<span style="font-family: \'schoolman\'">' + entity + '</span>' + html;
	}
	var icons = {
		'icon-discount': '&#xe900;',
		'icon-currency': '&#xe901;',
		'icon-printer': '&#xe902;',
		'icon-professor': '&#xe903;',
		'icon-student': '&#xe904;',
		'icon-user-config': '&#xe905;',
		'icon-home': '&#xe906;',
		'icon-school': '&#xe907;',
		'icon-bus': '&#xe908;',
		'icon-blackboard': '&#xe909;',
		'icon-list': '&#xe90a;',
		'icon-power': '&#xe90b;',
		'icon-settings': '&#xe90c;',
		'0': 0
		},
		els = document.getElementsByTagName('*'),
		i, c, el;
	for (i = 0; ; i += 1) {
		el = els[i];
		if(!el) {
			break;
		}
		c = el.className;
		c = c.match(/icon-[^\s'"]+/);
		if (c && icons[c[0]]) {
			addIcon(el, icons[c[0]]);
		}
	}
}());
