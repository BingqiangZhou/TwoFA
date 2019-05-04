#签名灵感来源于微信公众号二次开发签名#
##微信签名方式##
- 参数描述
- 
- 1.  signature	微信加密签名，signature结合了开发者填写的token参数和请求中的timestamp参数、nonce参数。
- 2.  timestamp	时间戳
- 3.  nonce		随机数
- 4.  echostr		随机字符串


- 生成签名的流程如下：
- 
- 1）将token、timestamp、nonce三个参数进行字典序排序
- 2）将三个参数字符串拼接成一个字符串进行sha1加密 
- 3）开发者获得加密后的字符串可与signature对比，标识该请求来源于微信

官方给出的检验signature的PHP示例代码：

    private function checkSignature()
    {
    _GET["signature"];
    _GET["timestamp"];
    _GET["nonce"];
    
    tmpArr = array(timestamp, $nonce);
    sort($tmpArr, SORT_STRING);
    $tmpStr = implode( $tmpArr );
    $tmpStr = sha1( $tmpStr );
    
    if( signature ){
    return true;
    }else{
    return false;
    }
    }

##我的签名方式

> 我使用了类似的方式，将参数镜像字典排序，但在这里我添加了一个timestamp（时间戳），用来控制有效时间（十分钟有效）
> 再者就是参数中key生成sign，而最后传参的时候不传key，直接传sign。