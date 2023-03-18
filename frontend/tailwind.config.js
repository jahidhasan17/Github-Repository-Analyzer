/** @type {import('tailwindcss').Config} */
module.exports = {
	content: [
		"./src/**/*.{js,jsx,ts,tsx}",
	],
  	theme: {
   		extend: {
			textUnderlineOffset: {
				16: '16px',
			},
			width: {
				'91': '91%',
				'81': '81%',
				'71': '71%',
				'61' : '61%',
				'51' : '51%',
				'46' : '46%',
				'42' : '42%',
				'41' : '41%',
				'39' : '39%',
				'35' : '35%',
				'31' : '31%',
				'27' : '27%',
				'timeline-button' : '70px',
			},
			fontSize: {
				'27': '27px',
				'22' : '22px',
				'21' : '21px',
				'18' : '19px',
				'5' : '18px',
			},
			fontWeight: {
				'more' : 2000
			},
			fontWeight: {
				'timeline-description' : 450,
			},
			height: {
				'timeline-button' : '70px',
			},
			margin: {
				'50': '50px',
				'300': '300px',
			}
		},
 	},
  	plugins: [],
}
