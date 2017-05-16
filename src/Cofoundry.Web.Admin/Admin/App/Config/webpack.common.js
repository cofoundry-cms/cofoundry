var webpack = require('webpack');
var HtmlWebpackPlugin = require('html-webpack-plugin');
var ExtractTextPlugin = require('extract-text-webpack-plugin');
var helpers = require('./helpers');

module.exports = {
  entry: {
    'polyfills': './polyfills.ts',
    'vendor': './vendor.ts',
    'app': './main.ts'
  },

  resolve: {
    extensions: ['.ts', '.js']
  },

  module: {
    rules: [
      // TYPESCRIPT
      {
        test: /\.ts$/,
        loaders: [
          {
            loader: 'awesome-typescript-loader',
            options: { configFileName: helpers.root('tsconfig.json') }
          } , 'angular2-template-loader'
        ]
      },
      // HTML
      {
        test: /\.html$/,
        loader: 'html-loader'
      },
      // FILES
      {
        test: /\.(png|jpe?g|gif|svg|woff|woff2|ttf|eot|ico)$/,
        loader: 'file-loader?name=assets/[name].[hash].[ext]'
      },
      // SASS
      {
        test: /\.scss$/,
        exclude: helpers.root('modules'),
        loader: ExtractTextPlugin.extract({ fallbackLoader: 'style-loader', loader: 'css-loader!sass-loader?sourceMap' })
      },
      {
        test: /\.scss$/,
        include: helpers.root('modules'),
        loader: 'raw-loader!sass-loader'
      },
      // FONTS
      {
        test: /.(woff(2)?|eot|ttf|svg)(\?[a-z0-9=\.]+)?$/,
        loader: 'url-loader?limit=1&name=[name].[ext]'
      },
      // IMAGES
      {
        test: /\.(jpe?g|png|gif|svg)/,
        loader: 'url-loader?limit=1&name=[name].[ext]'
      },
      // VIDEOS
      {
        test: /\.(mp4)/,
        loader: 'url-loader?limit=1&name=[name].[ext]'
      },
      // PDFs
      {
        test: /\.(pdf)/,
        loader: 'url-loader?limit=1&name=[name].[ext]'
      }
    ]
  },

  plugins: [
    // Workaround for angular/angular#11580
    new webpack.ContextReplacementPlugin(
      // The (\\|\/) piece accounts for path separators in *nix and Windows
      /angular(\\|\/)core(\\|\/)(esm(\\|\/)src|src)(\\|\/)linker/,
      helpers.root('../'), // location of your src
      {} // a map of your routes
    ),

    new webpack.optimize.CommonsChunkPlugin({
      name: ['app', 'vendor', 'polyfills']
    }),
    /*
    new HtmlWebpackPlugin({
      template: 'src/index.html'
    })
    */
  ]
};

