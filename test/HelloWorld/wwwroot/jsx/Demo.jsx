var Demo = React.createClass({
    getInitialState: function() {
        return {secondsElapsed: 0};
    },
    tick: function() {
        this.setState({secondsElapsed: this.state.secondsElapsed + 1});
    },
    componentDidMount: function() {
        this.interval = setInterval(this.tick, 1000);
    },
    componentWillUnmount: function() {
        clearInterval(this.interval);
    },
    render: function() {
        return (
            <div>
                <h1>
                DEMO!!!
                    <span dangerouslySetInnerHTML={{__html: this.props.title}}></span> 
                    : {this.state.secondsElapsed}
                </h1>
            </div>
        );
    }
});
