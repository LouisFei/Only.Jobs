//======================================================================
// Copyright (c) Only.Jobs  development team. All rights reserved.
// 所属项目：Only.Jobs js.controller
// 创 建 人：Only.Jobs development team
// 创建日期：2017-07-06 16:21:57
// 用    途：后台任务 (BackgroundJob)js控制器
//======================================================================


/**
*后台任务新增视图控制器
**/
function BackgroundJobAddController($scope, $http, $window) {
    $scope.model = {
        BackgroundJobId: EmptyGUID,
        JobType: '',
        Name: '',
        AssemblyName: '',
        ClassName: '',
        Description: '',
        JobArgs: '',
        CronExpression: '',
        CronExpressionDescription: '',
        NextRunTime: '',
        LastRunTime: '',
        RunCount: '',
        State: '0',
        DisplayOrder: '0',
    };
    $scope.SubmitAdd = function () {
        if (!$('#form').validationEngine('validate')) {
            return;
        }
        $http({
            method: 'POST',
            url: '/BackgroundJob/AddPost?r=' + Math.random(),
            data: $scope.model
        })
       .success(function (result, status, headers, config) {

           if (result.success) {
               oUtils.alertSuccess('保存成功', function () {
                   $("#opt_is_success").val('true');
                   parent.layer.close(parent.layer.getFrameIndex(window.name));
               });
           } else {
               oUtils.alertError('保存失败');
           }
       })
       .error(function (result, status, headers, config) {
           alert(result.message || '操作过程中发生异常');
       });
    };
};

/**
*后台任务修改视图控制器
**/
function BackgroundJobUpdateController($scope, $http, $window) {

    var request = oUtils.urlParameter();
    var BackgroundJobId = request["BackgroundJobId"];

    $scope.model = {
        BackgroundJobId: BackgroundJobId,
        JobType: '',
        Name: '',
        AssemblyName: '',
        ClassName: '',
        Description: '',
        JobArgs: '',
        CronExpression: '',
        CronExpressionDescription: '',
        NextRunTime: '',
        LastRunTime: '',
        RunCount: '',
        State: '',
        DisplayOrder: '',
    };

    $scope.LoadBackgroundJobInfo = function () {
        $scope.model.BackgroundJobId = BackgroundJobId;
        $http({
            method: 'GET',
            url: '/BackgroundJob/InfoData?r=' + Math.random(),
            cache: false,
            params: { BackgroundJobId: BackgroundJobId }
        })
       .success(function (result, status, headers, config) {
           if (result.success) {
               $scope.model = result.data;
           } else {
               oUtils.alertError(result.message);
           }
       })
       .error(function (result, status, headers, config) {
           alert(result.message || '请求过程中发生异常');
       });
    };

    $scope.SubmitUpdate = function () {
        if (!$('#form').validationEngine('validate')) {
            return;
        }
        $http({
            method: 'POST',
            url: '/BackgroundJob/UpdatePost?r=' + Math.random(),
            data: $scope.model
        })
        .success(function (result, status, headers, config) {
            if (result.success) {
                oUtils.alertSuccess('保存成功', function () {
                    $("#opt_is_success").val('true');
                    parent.layer.close(parent.layer.getFrameIndex(window.name));
                });
            } else {
                oUtils.alertError('保存失败');
            }
        })
        .error(function (result, status, headers, config) {
            alert(result.message || '请求过程中发生异常');
        });
    };

    $(function () {
        $scope.LoadBackgroundJobInfo();
    });
};
