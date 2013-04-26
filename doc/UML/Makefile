all: Overall.png Network.png Filesystem.png

Overall.png: Overall.dot
	dot -Tpng Overall.dot -o $@

Network.png: Network.dot
	dot -Tpng Network.dot -o $@

Filesystem.png: Filesystem.dot
	dot -Tpng Filesystem.dot -o $@

clean:
	rm -f *.png
