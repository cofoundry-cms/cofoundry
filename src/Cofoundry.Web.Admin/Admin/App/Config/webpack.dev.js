var webpackMerge = require('webpack-merge');
var ExtractTextPlugin = require('extract-text-webpack-plugin');
var commonConfig = require('./webpack.common.js');
var helpers = require('./helpers');

module.exports = webpackMerge(commonConfig, {
  devtool: 'cheap-module-eval-source-map',
  
  watch: true,
  
  output: {
    path: helpers.root('bundles'),
    filename: '[name].bundle.js',
    chunkFilename: '[id].chunk.js',
    publicPath: helpers.root('bundles')
  },
  
  plugins: [
    new ExtractTextPlugin('[name].css')
  ]
  
});
