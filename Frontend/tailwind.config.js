module.exports = {
  content: [
    './index.html',
    './src/**/*.{vue,js,ts,jsx,tsx}',
  ],
  theme: {
    extend: {
      colors: {
        'cz-red': {
          '50' : '#ffeff1',
          '100' : '#ffdce0',
          '200' : '#ffbfc7',
          '300' : '#ff929f',
          '400' : '#ff5468',
          '500' : '#ff1f39',
          '600' : '#ff001e',
          '700' : '#db001a',
          '800' : '#b30015', //main red
          '900' : '#940818',
          '950' : '#52000a',
        },
        'cz-background': {
          '700' : 'rgba(24, 24, 24, 0.8)',
          '800' : '#181818',
          '900' : '#1F1F1F',
        }
      },

      spacing: {
        '5px' : '5px',
        '20px' : '20px',
        '19': '4.75rem'
      },
    },
  },
  plugins: [],
}
