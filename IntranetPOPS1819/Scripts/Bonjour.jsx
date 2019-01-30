class CommentList extends React.Component {
    render() {
        return (
            <div className="commentList">
                <Comment author="Nathan Bonnard">
                    Je  *pete* regulierement sur les gens
                </Comment>
                <Comment author="Brian ">Je suis Brian et je suis musclé</Comment>
                <Comment author="Mahitas">
                    Salut j'ai fait une faute à mon propre nom
                </Comment>
            </div>
        );
    }
}

class Comment extends React.Component {
    rawMarkup() {
        const md = new Remarkable();
        const rawMarkup = md.render(this.props.children.toString());
        return { __html: rawMarkup };
    }
    render() {
   
        return (
            <div className="comment">
                <h2 className="commentAuthor">{this.props.author}</h2>
                <span dangerouslySetInnerHTML={this.rawMarkup()} />
            </div>
        );
    }
}

class CommentForm extends React.Component {
    render() {
        return (
            <div className="commentForm">Hello, world! I am a CommentForm.</div>
        );
    }
}
class CommentBox extends React.Component {
    render() {
        return (
            <div className="commentBox">
                <h1>Comments</h1>
                <CommentList />
                <CommentForm />
            </div>
        );
    }
}

ReactDOM.render(<CommentBox />, document.getElementById('content'));