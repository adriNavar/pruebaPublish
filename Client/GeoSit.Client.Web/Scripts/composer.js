function LayerComposer() {
    this.pre = {
        handler: function (event) {
            var ctx = event.context;
            var width = ctx.canvas.width * ($("#swipe").val() / 100);

            ctx.save();
            ctx.beginPath();
            ctx.rect(width, 0, ctx.canvas.width - width, ctx.canvas.height);
            ctx.clip();
        }
    };
    this.post = {
        handler: function (event) {
            var ctx = event.context;
            ctx.restore();
        }
    };
}