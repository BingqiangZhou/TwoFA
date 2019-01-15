var cryptoJs = require('crypto-js/crypto-js.js')

//获取验证码
function GetCode(skey, stext) {
      //console.log(skey + stext);
      var key = cryptoJs.HmacSHA1(skey, stext);
      var bytes = [];
      for (var c = 0; c < key.sigBytes * 2; c += 2) {
        bytes.push(parseInt(key.toString().toUpperCase().substr(c, 2), 16));
        //console.log(parseInt(key.toString().substr(c, 2), 16));
      }
      var offset = bytes[19] & 0xf;
      var v = (bytes[offset] & 0x7f) << 24 |
        (bytes[offset + 1] & 0xff) << 16 |
        (bytes[offset + 2] & 0xff) << 8 |
        (bytes[offset + 3] & 0xff);

      v = v + '';
      var code = parseInt(v) % (10e5);
      //不足6位的code补位6位，在后面补0
      return code*Math.pow(10,6-code.toString().length);
    }
//获取步数
function GetCounter(){
  var date = new Date();
  return Math.floor(date.getTime() / (30*1000));
}

function GetCountdown(){
  var date = new Date();
  return Math.ceil(date.getSeconds()%30 * (100/30));
}

module.exports = {
  GetCode: GetCode,
  GetCounter: GetCounter,
  GetCountdown: GetCountdown
}