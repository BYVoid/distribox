all: Overall.png Network.png Filesystem.png Version.png

Overall.png: Overall.dot
	dot -Tpng Overall.dot -o $@

Network.png: Network.dot
	dot -Tpng Network.dot -o $@

Filesystem.png: Filesystem.dot
	dot -Tpng Filesystem.dot -o $@

Version.png: Version.dot
	dot -Tpng Version.dot -o $@

clean:
	rm -f *.png
