/**
 * @name mrc 2018-4-6
 * @description 表单校验 依赖Jq 改写自layui form
 */
;
! function($) {
	"use strict";
	var MOD_NAME = 'form';
	var ELEM = '.mrc-form';
	var eventConfig = {
			  event: {} //记录模块自定义事件
			 ,blurinit:false//记录失去焦点事件
	}
	
    //选择器对象
	var selectorModule = function(){};   

	//构建验证器对象
	var FV = function() {};
	
	//构建哈希表对象
	var  HashTable=function(){
    // 初始化哈希表的记录条数size
    var size = 0;

    // 创建对象用于接受键值对
    var res = {};

    // 添加关键字，无返回值
    this.add = function (key, value) {

        //判断哈希表中是否存在key，若不存在，则size加1，且赋值 
        if (!this.containKey(key)) {
            size++;
        }

        // 如果之前不存在，赋值； 如果之前存在，覆盖。
        res[key] = value;
    };

    // 删除关键字, 如果哈希表中包含key，并且delete返回true则删除，并使得size减1
    this.remove = function (key) {
        if (this.containKey(key) && (delete res[key])) {
            size--;
        }
    };

    // 哈希表中是否包含key，返回一个布尔值
    this.containKey = function (key) {
        return (key in res);
    };

    // 哈希表中是否包含value，返回一个布尔值
    this.containValue = function (value) {

        // 遍历对象中的属性值，判断是否和给定value相等
        for (var prop in res) {
            if (res[prop] === value) {
                return true;
            }
        }
        return false;
    };

    // 根据键获取value,如果不存在就返回null
    this.getValue = function (key) {
        return this.containKey(key) ? res[key] : null;
    };

    // 获取哈希表中的所有value, 返回一个数组
    this.getAllValues = function () {
        var values = [];
        for (var prop in res) {
            values.push(res[prop]);
        }
        return values;
    };

    // 根据值获取哈希表中的key，如果不存在就返回null
    this.getKey = function (value) {
        for (var prop in res) {
            if (res[prop] === value) {
                return prop;
            }
        }

        // 遍历结束没有return，就返回null
        return null;
    };

    // 获取哈希表中所有的key,返回一个数组
    this.getAllKeys = function () {
        var keys = [];
        for (var prop in res) {
            keys.push(prop);
        }
        return keys;
    };

    // 获取哈希表中记录的条数，返回一个数值
    this.getSize = function () {
        return size;
    };

    // 清空哈希表，无返回值
    this.clear = function () {
        size = 0;
        res = {};
    };
}
	
    //构建表单对象
	var Form = function() {
		this.config = {
			verify: {
				required: [
					/[\S]+/, '必填项不能为空'
				],
				phone: [
					/^1\d{10}$/, '请输入正确的手机号'
				],
				email: [
					/^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/, '邮箱格式不正确'
				],
				url: [
					/(^#)|(^http(s*):\/\/[^\s]+\.[^\s]+)/, '链接格式不正确'
				],
				number: function(value) {
					if(!value || isNaN(value)) return '只能填写数字'
				},
				date: [
					/^(\d{4})[-\/](\d{1}|0\d{1}|1[0-2])([-\/](\d{1}|0\d{1}|[1-2][0-9]|3[0-1]))*$/, '日期格式不正确'
				],
				identity: [
					/(^\d{15}$)|(^\d{17}(x|X|\d)$)/, '请输入正确的身份证号'
				],
				UserName: [
					/^[a-zA-Z]{1}([a-zA-Z0-9]|[._]){4,19}$/, '只能输入5-20个以字母开头、可带数字、"_"、"."的字串'
				],
				IP: [

					/^(?:(?:2[0-4][0-9]\.)|(?:25[0-5]\.)|(?:1[0-9][0-9]\.)|(?:[1-9][0-9]\.)|(?:[0-9]\.)){3}(?:(?:2[0-5][0-5])|(?:25[0-5])|(?:1[0-9][0-9])|(?:[1-9][0-9])|(?:[0-9]))$/, '请输入正确的IP地址'
				],
				Password: [
					/^[a-zA-Z\d_]{8,}$/, '密码限制为8位以上'
				],
				Chinese: [
					/^[\u4e00-\u9fa5]+$/, '只允许输入1-5个中文' //{1,5}限制多少个字符
				],
				Pnumber: [
					/^\d{6}$/, '请输入6位数字的邮政编号'
				]
			}
		};
	};

    //构建基础功能对象
	var baseTool = function() {
			//遍历
			this.each = function(obj, fn) {
					var key, that = this;
					if(typeof fn !== 'function') return that;
					obj = obj || [];
					if(obj.constructor === Object) {
						for(key in obj) {
							if(fn.call(obj[key], key, obj[key])) break;
						}
					} else {
						for(key = 0; key < obj.length; key++) {
							if(fn.call(obj[key], key, obj[key])) break;
						}
					}
					return that;
				},
				//获取设备信息
				this.device = function(key) {
					var agent = navigator.userAgent.toLowerCase(),
						getVersion = function(label) {
							var exp = new RegExp(label + '/([^\\s\\_\\-]+)');
							label = (agent.match(exp) || [])[1];
							return label || false;
						}
						//返回结果集					
						,
						result = {
							//底层操作系统
							os: function() {
									if(/windows/.test(agent)) {
										return 'windows';
									} else if(/linux/.test(agent)) {
										return 'linux';
									} else if(/iphone|ipod|ipad|ios/.test(agent)) {
										return 'ios';
									} else if(/mac/.test(agent)) {
										return 'mac';
									}
								}()
								//ie版本
								,
							ie: function() {
									return(!!window.ActiveXObject || "ActiveXObject" in window) ? (
										(agent.match(/msie\s(\d+)/) || [])[1] || '11' //由于ie11并没有msie的标识
									) : false;
								}()
								//是否微信
								,
							weixin: getVersion('micromessenger')
						};
					//任意的key
					if(key && !result[key]) {
						result[key] = getVersion(key);
					}
					//移动设备
					result.android = /android/.test(agent);
					result.ios = result.os === 'ios';
					return result;
				}
			this.getEelem = function(selector, result) {
				result = result || [];
				var rquickExpr = /^(?:#([\w-]+)|\.([\w-]+)|([\w]+)|(\*))$/,
					m = rquickExpr.exec(selector);
				if(!m) {
					return result;
				}
				if(m[1]) {
					result = selectormodule.getId(selector);
				} else if(m[2]) {
					result = selectormodule.getClass(selector);
				} else if(m[3]) {
					result = selectormodule.getTag(selector);
				}
				return result;
			}

		}
    
    //模块对象		
	var eventDefined = function() {
		this.event = function(modName, events, params) {
				var that = this,
					result = null,
					filter = events.match(/\(.*\)$/) || [] //提取事件过滤器
					,
					set = (events = modName + '.' + events).replace(filter, '') //获取事件本体名
					,
					callback = function(_, item) {
						var res = item && item.call(that, params);
						res === false && result === null && (result = false);
					};
				basetool.each(eventConfig.event[set], callback);
				filter[0] && basetool.each(eventConfig.event[events], callback); //执行过滤器中的事件
				return result;
			},
			this.onevent = function(modName, events, callback) {
				if(typeof modName !== 'string' ||
					typeof callback !== 'function') return this;
				eventConfig.event[modName + '.' + events] = [callback];
				return this;
			}
	}
    
   
	FV.prototype.verifyThis = function(thisElem) {
			var verify = form.config.verify;
			var eventlist = [];
			var othis = thisElem, // basetool.getEelem(thisElem),	
				vers = othis.attr('mrc-verify').split('|'),
				verType = othis.attr('mrc-verType'),
				value = othis.val();
			basetool.each(vers, function(_, thisVer) {
				var isTrue //是否命中校验
					, errorText = '' //错误提示文本
					,
					isFn = typeof verify[thisVer] === 'function';

				//匹配验证规则
				if(verify[thisVer]) {
					var isTrue = isFn ? errorText = verify[thisVer](value, function() {
						return true
					}) : !verify[thisVer][0].test(value);
					errorText = errorText || verify[thisVer][1];
					//如果是必填项或者非空命中校验
					if(isTrue) {
						var em = {
							elem: othis,
							msg: errorText
						};
						eventlist.push(em);
						//if(!device.android && !device.ios) item.focus(); //非移动设备自动定位焦点
					}
				}
			});
			return eventlist;
	 }
		
	FV.prototype.FormVerify = function(thisEelem,callback)
	{
			var verify = form.config.verify,
				stop = null,
				field = {},
				elem = basetool.getEelem(thisEelem),
				verifyElem = $(thisEelem).find('*[mrc-verify]') //获取需要校验的元素
				,
				fieldElem = $(thisEelem).find('input,select,textarea'); //获取所有表单域

			var eventlist = [];
			//开始校验
			basetool.each(verifyElem, function(_, item) {
				var othis = $(this),
					vers = othis.attr('mrc-verify').split('|'),
					verType = othis.attr('mrc-verType'),
					value = othis.val();
				basetool.each(vers, function(_, thisVer) {
					var isTrue //是否命中校验
						, errorText = '' //错误提示文本
						,
						isFn = typeof verify[thisVer] === 'function';
					//匹配验证规则
					if(verify[thisVer]) {
						var isTrue = isFn ? errorText = verify[thisVer](value, item) : !verify[thisVer][0].test(value);
						errorText = errorText || verify[thisVer][1];
						//如果是必填项或者非空命中校验
						if(isTrue) {
							var em = {
								elem: othis,
								msg: errorText
							};
							eventlist.push(em);						 
						}
					}
				});
			});

			var nameIndex = {}; //数组 name 索引
			basetool.each(fieldElem, function(_, item) {
				item.name = (item.name || '').replace(/^\s*|\s*&/, '');
				if(!item.name) return;
				//用于支持数组 name
				if(/^.*\[\]$/.test(item.name)) {
					var key = item.name.match(/^(.*)\[\]$/g)[0];
					nameIndex[key] = nameIndex[key] | 0;
					item.name = item.name.replace(/^(.*)\[\]$/, '$1[' + (nameIndex[key]++) + ']');
				}
				if(/^checkbox|radio$/.test(item.type) && !item.checked) return;
				field[item.name] = item.value;
			});
			eventlist.push(field);
			//获取字段
		    return eventdefined.onevent.call(this, MOD_NAME, 'hello', callback);
		   
		    return eventlist;
	}
    
    FV.prototype.on=function(events, callback){    	
        return eventdefined.onevent.call(this, MOD_NAME, events, callback);    
    }


	//通过id获取元素
	selectorModule.prototype.getId = function(id, result) {
			result = result || [];
			//  由于获取的Id是一个元素,所以这里使用call.
			result.push.call(result, document.getElementById(id.replace('#', '')));
			return result;
		}
		//通过class获取元素
	selectorModule.prototype.getClass = function(className, result) {
			result = result || [];
			result.push.apply(result, document.getElementsByClassName(className.replace('.', '')));
			return result;
		}
		//通过Tag获取元素
	selectorModule.prototype.getTag = function(tagName, result) {
			result = result || [];
			result.push.apply(result, document.getElementsByTagName(tagName));
			return result;
		}
	
	//表单全局设置
	Form.prototype.set = function(options) {
		var that = this;
		$.extend(true, that.config, options);
		return that;
	};
	//表单验证规则设定
	Form.prototype.verify = function(settings) {
		var that = this;
		$.extend(true, that.config.verify, settings);
		return that;
	};
	//表单事件监听
	Form.prototype.on = function(events, callback)
	{
		return eventdefined.onevent.call(this, MOD_NAME, events, callback);
	};
    //表单事件绑定
	Form.prototype.blurEvent = function(thisSelector,callback){			
		    var em='#'+thisSelector.match(/\(.*\)$/);
	     	em=em.replace("(",'').replace(")",'')
			var verifyElem = $(em).find('*[mrc-verify]');
			var result=[];			
			basetool.each(verifyElem, function(_, item) {
			var othis = $(this);
			$(othis).attr('mrc-own',em);
			});		
			if(!eventConfig.blurinit)
			{
				init();		
				eventConfig.blurinit=true;
			}			
			return eventdefined.onevent.call(this, MOD_NAME, thisSelector, callback);
	}    
	var eventdefined = new eventDefined();
	//表单提交校验
	var submitWithSelector = function(thisEelem) {
		var verify = form.config.verify,
			stop = null,
			field = {},
			elem = basetool.getEelem(thisEelem),
			verifyElem = elem.find('*[mrc-verify]') //获取需要校验的元素
			,
			fieldElem = elem.find('input,select,textarea'); //获取所有表单域

		var eventlist = [];
		//开始校验
		basetool.each(verifyElem, function(_, item) {
			var othis = $(this),
				vers = othis.attr('mrc-verify').split('|'),
				verType = othis.attr('mrc-verType'),
				value = othis.val();
			basetool.each(vers, function(_, thisVer) {
				var isTrue //是否命中校验
					, errorText = '' //错误提示文本
					,
					isFn = typeof verify[thisVer] === 'function';
				//匹配验证规则
				if(verify[thisVer]) {
					var isTrue = isFn ? errorText = verify[thisVer](value, item) : !verify[thisVer][0].test(value);
					errorText = errorText || verify[thisVer][1];
					//如果是必填项或者非空命中校验
					if(isTrue) {
						var em = {
							elem: othis,
							msg: errorText
						};
						eventlist.push(em);
						//if(!device.android && !device.ios) item.focus(); //非移动设备自动定位焦点
					}
				}
			});
		});

		var nameIndex = {}; //数组 name 索引
		basetool.each(fieldElem, function(_, item) {
			item.name = (item.name || '').replace(/^\s*|\s*&/, '');
			if(!item.name) return;
			//用于支持数组 name
			if(/^.*\[\]$/.test(item.name)) {
				var key = item.name.match(/^(.*)\[\]$/g)[0];
				nameIndex[key] = nameIndex[key] | 0;
				item.name = item.name.replace(/^(.*)\[\]$/, '$1[' + (nameIndex[key]++) + ']');
			}
			if(/^checkbox|radio$/.test(item.type) && !item.checked) return;
			field[item.name] = item.value;
		});
		eventlist.push(field);
		return evenlist;
	};

	//表单提交校验
	var submit = function() {
		var button = $(this),
			verify = form.config.verify,
			stop = null,
			DANGER = 'mrc-form-danger',
			field = {},
			elem = button.parents(ELEM)
		,
		verifyElem = elem.find('*[mrc-verify]') //获取需要校验的元素
			,
			formElem = button.parents('form')[0] //获取当前所在的form元素，如果存在的话
			,
			fieldElem = elem.find('input,select,textarea') //获取所有表单域
			,
			filter = button.attr('mrc-filter'); //获取过滤器
		var eventlist = [];
		//开始校验
		basetool.each(verifyElem, function(_, item) {
			var othis = $(this),
				vers = othis.attr('mrc-verify').split('|'),
				verType = othis.attr('mrc-verType'),
				value = othis.val();
			othis.removeClass(DANGER);
			basetool.each(vers, function(_, thisVer) {
				var isTrue //是否命中校验
					, errorText = '' //错误提示文本
					,
					isFn = typeof verify[thisVer] === 'function';
				//匹配验证规则
				if(verify[thisVer]) {
					var isTrue = isFn ? errorText = verify[thisVer](value, item) : !verify[thisVer][0].test(value);
					errorText = errorText || verify[thisVer][1];
					//如果是必填项或者非空命中校验
					if(isTrue) {
						var em = {
							elem: othis,
							msg: errorText
						};
						eventlist.push(em);
						if(!device.android && !device.ios) item.focus(); //非移动设备自动定位焦点
						othis.addClass(DANGER);
						return stop = true;
					}
				}
			});
			if(stop) return stop;
		});
		if(stop) {
			//获取字段
			return eventdefined.event.call(this, MOD_NAME, 'submit(' + filter + ')', {
				eventlist: eventlist
					//return false;
			});
		}
		var nameIndex = {}; //数组 name 索引
		basetool.each(fieldElem, function(_, item) {
			item.name = (item.name || '').replace(/^\s*|\s*&/, '');

			if(!item.name) return;

			//用于支持数组 name
			if(/^.*\[\]$/.test(item.name)) {
				var key = item.name.match(/^(.*)\[\]$/g)[0];
				nameIndex[key] = nameIndex[key] | 0;
				item.name = item.name.replace(/^(.*)\[\]$/, '$1[' + (nameIndex[key]++) + ']');
			}

			if(/^checkbox|radio$/.test(item.type) && !item.checked) return;
			field[item.name] = item.value;
		});
		//获取字段
		return eventdefined.event.call(this, MOD_NAME, 'submit(' + filter + ')', {
			elem: this,
			form: formElem,
			field: field,
			eventlist: eventlist
		});
	};

    var blurVerify=function(){   	    
    	    var verify = form.config.verify;
			var eventlist = [];
			var othis = this,	
				vers = $(othis).attr('mrc-verify').split('|'),
				verType = $(othis).attr('mrc-verType'),
				value = $(othis).val(),
				filter= $(othis).attr('mrc-own');
			basetool.each(vers, function(_, thisVer) {
				var isTrue //是否命中校验
					, errorText = '' //错误提示文本
					,
					isFn = typeof verify[thisVer] === 'function';

				//匹配验证规则
				if(verify[thisVer]) {
					var isTrue = isFn ? errorText = verify[thisVer](value, function() {
						return true
					}) : !verify[thisVer][0].test(value);
					errorText = errorText || verify[thisVer][1];
					//如果是必填项或者非空命中校验
					if(isTrue)
					{
						var em = {
							elem: othis,
							msg: errorText
						};
						eventlist.push(em);
					}
				}
			});
		
		//window.console.log(eventlist);
		
		//获取字段
		return eventdefined.event.call(this,MOD_NAME,'blur('+filter.replace("#",'').replace('.','')+')', {
			elem: this,
			eventlist: eventlist
		});
    }
     

	//实例化
	var form = new Form();
	var dom = $(document);
	var basetool = new baseTool();
	var device = basetool.device();
	var selectormodule = new selectorModule();
	var fv = new FV();
	var hashTable=new HashTable();
		
	//表单提交事件
	dom.on('submit', ELEM, submit)
		.on('click', '*[mrc-submit]', submit);
		
	//失去焦点事件	
	function init()
	{		
		dom.on('blur', '*[mrc-own]', blurVerify);			
	}

		
	//暴露表单验证模块
	window.mrcForm = form;
	//暴露基本功能模块
	window.mrcBaseTool = basetool;
    //暴露哈希表功能模块
    window.mrcHT=hashTable;
    //暴露
	window.fv = fv;
		
}(jQuery);