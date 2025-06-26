const app = {
    state: {
        isCollapse: false, //控制菜单的展开还是收起
        roleList:[], //职位配置
        menu:[], //菜单
    },
    mutations: {
        //修改菜单展开收起的方法
        collapseMenu(state) {
            state.isCollapse = !state.isCollapse
        },
        initRoleList(state,data){
            state.roleList = data;
        },
        SET_MENU(state, menu){
            state.menu = menu;
        }
    },
    actions:{}
};

export default app;