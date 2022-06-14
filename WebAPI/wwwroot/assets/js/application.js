(() => {
    $("#show-admin-token").on("click", ()=> {
        let input = $("#admin-token")
        input.attr("type", input.attr("type") == "password" ? "text" : "password")
    })
    PopulateAccounts()
    function PopulateAccounts(){
        let list = $("#account-section .list")[0]
    }
}).call()